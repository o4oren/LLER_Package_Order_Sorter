using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

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
                        appData + "\\Packages\\Microsoft Flight Simulator\\content.xml";
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
                    XmlNode llerNode = prioritiesNode.SelectSingleNode("//Package[@name='ftxdes-airport_ller-eilat']");

                    if (navdataNode != null)
                    {
                        int navdataPriority = int.Parse(navdataNode.Attributes["priority"].Value);

                        // If llerNode exists and it's priority is lower than navdata, do nothing and exit
                        if (llerNode != null)
                        {
                            if (int.Parse(llerNode.Attributes["priority"].Value) >= navdataPriority)
                            {
                                Console.WriteLine("Deleting LLER node.");
                                prioritiesNode.RemoveChild(llerNode);
                            }
                            else
                            {
                                Console.WriteLine("Found LLER at a lower level than Navigraph. Exiting.");
                                return;
                            }
                        }

                        // Update the priority higher priority nodes
                            foreach (XmlNode node in rootNode.SelectNodes("//Package"))
                        {
                            int nodePriority = int.Parse(node.Attributes["priority"].Value);
                            if (nodePriority >= navdataPriority)
                            {
                                node.Attributes["priority"].Value = (nodePriority + 1).ToString();
                            }
                        }

                        // Create a new entry for LLER node
                        XmlElement llerElement = xmlDoc.CreateElement("Package");
                        llerElement.SetAttribute("name", "ftxdes-airport_ller-eilat");
                        llerElement.SetAttribute("priority", (navdataPriority).ToString()); // Adjust priority

                        // Insert 'ftxdes-airport_ller-eilat' at the original level of 'navdata'
                        rootNode.InsertBefore(llerElement, navdataNode);

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
