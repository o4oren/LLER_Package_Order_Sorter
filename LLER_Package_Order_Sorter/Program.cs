using System;

using System.Xml;

namespace LLER_Package_Order_Sorter
{
    internal class Program
    {
        static void Main()
        {
            // Path to content.xml in MSFS
            string contentXmlPath = "path/to/MSFS/content.xml";

            try
            {
                // Load the content.xml
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(contentXmlPath);

                // Find the root node
                XmlNode rootNode = xmlDoc.DocumentElement;

                // Find the 'navdata' node and get its priority
                XmlNode navdataNode = rootNode.SelectSingleNode("//entry[@name='navdata']");
                int navdataPriority = int.Parse(navdataNode.Attributes["priority"].Value);

                // Create a new entry 'test-entry' node
                XmlElement testEntryNode = xmlDoc.CreateElement("entry");
                testEntryNode.SetAttribute("name", "test-entry");
                testEntryNode.SetAttribute("priority", (navdataPriority - 1).ToString()); // Adjust priority

                // Insert 'test-entry' at the same level as 'navdata'
                rootNode.InsertBefore(testEntryNode, navdataNode);

                // Update the priority of 'navdata' and higher priority nodes
                foreach (XmlNode node in rootNode.SelectNodes("//entry"))
                {
                    int nodePriority = int.Parse(node.Attributes["priority"].Value);
                    if (nodePriority >= navdataPriority)
                    {
                        node.Attributes["priority"].Value = (nodePriority + 1).ToString();
                    }
                }

                // Save the changes back to content.xml
                xmlDoc.Save(contentXmlPath);

                Console.WriteLine("Updated content.xml in MSFS.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
