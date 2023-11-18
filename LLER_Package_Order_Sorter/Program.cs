using System;
using System.IO;
using System.Xml;

namespace LLER_Package_Order_Sorter
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                // Find the path to content.xml in MSFS
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var contentXmlPath = localAppData +
                                     "\\Packages\\Microsoft.FlightSimulator_8wekyb3d8bbwe\\LocalCache\\content.xml";
                if (!File.Exists(contentXmlPath))
                {
                    var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    contentXmlPath =
                        appData + "Packages\\Microsoft.FlightSimulator_8wekyb3d8bbwe\\LocalCache\\content.xml";
                    if (!File.Exists(contentXmlPath))
                    {
                        throw new Exception("Cannot find Content.xml!");
                    }
                }

                Console.WriteLine($"Found Content.xml at {contentXmlPath}");


                // Load the content.xml
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(contentXmlPath);

                // Find the root node
                XmlNode rootNode = xmlDoc.DocumentElement;

                XmlNode prioritiesNode = xmlDoc.SelectSingleNode("Priorities");
                if (prioritiesNode != null)
                {
                    // Find the 'navdata' node inside 'priorities'
                    XmlNode navdataNode = prioritiesNode.SelectSingleNode("//Package[@name='navigraph-navdata']");
                    if (navdataNode != null)
                    {
                        int navdataPriority = int.Parse(navdataNode.Attributes["priority"].Value);

                        // Update the priority higher priority nodes
                        foreach (XmlNode node in rootNode.SelectNodes("//Package"))
                        {
                            int nodePriority = int.Parse(node.Attributes["priority"].Value);
                            if (nodePriority > navdataPriority && !node.Attributes["name"].Value.Equals("navigraph-navdata"))
                            {
                                node.Attributes["priority"].Value = (nodePriority + 1).ToString();
                            }
                        }

                        // update the priority of the navdata node
                        navdataNode.Attributes["priority"].Value = navdataPriority.ToString();

                        // Create a new entry for LLER node
                        XmlElement llerNode = xmlDoc.CreateElement("Package");
                        llerNode.SetAttribute("name", "ftxdes-airport_ller-eilat");
                        llerNode.SetAttribute("priority", (navdataPriority - 1).ToString()); // Adjust priority

                        // Insert 'test-entry' at the original level of 'navdata'
                        rootNode.InsertBefore(llerNode, navdataNode);



                        // Save the changes back to content.xml
                        xmlDoc.Save(contentXmlPath);

                        Console.WriteLine("Updated content.xml in MSFS.");
                    }
                    else
                    {
                        Console.WriteLine("Navigraph Navdata node doesn't exist in Content.xml.");
                    }
                }
                else
                {
                    throw new Exception("Cannot find Priorities node!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                Console.Write("Press Enter to close window ...");
                Console.Read();
            }
        }
    }
}
