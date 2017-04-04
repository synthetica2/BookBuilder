﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookBuilder
{
    /// <summary>Stores information for a BB_Book including its BB_Pages, authors, etc.</summary>
    public class BB_Book
    {
        /// <summary>
        /// Gets the pages of the book.
        /// </summary>
        /// <value>The pages of the book.</value>
        public List<BB_Page> Pages { get; } = new List<BB_Page>();

        /// <summary>
        /// Gets the author(s) of the book.
        /// </summary>
        /// <value>The author(s) of the book.</value>
        public List<string> Authors { get; } = new List<string>();

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>The creation date.</value>
        public string CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name of the button image.
        /// </summary>
        /// <value>The name of the button image.</value>
        public string ButtonImageName { get; set; }

        /// <summary>
        /// Gets or sets the file version.
        /// </summary>
        /// <value>The file version.</value>
        public string FileVersion { get; set; }

        /// <summary>
        /// Checks if two pages that will be open at the same time both have an audio file. 
        /// If they do a warning is displayed to the user (for now just write to console).
        /// </summary>
        public void AudioFileCheck()
        {
            if (Pages.Count < 2)
            {
                return;
            }
            for (int i = 0; i < Pages.Count; i += 2)
            {
                BB_Page leftPage = Pages[i];
                BB_Page rightPage = Pages[i + 1];
                if (leftPage.AudioFileName != null && rightPage.AudioFileName != null)
                {
                    Console.WriteLine("Warning: Page {0} and Page {1} both have audio files and will be open at the same time",
                                      i, i + 1);
                    //TODO: Make this a dialog box popup instead. Maybe by having AudioFileCheck return a bool, which the GUI would check
                    //when creating the book.
                }
            }
        }

        /// <summary>Creates a zip file of the books data (pages, videos, etc.) and config.xml.</summary>
		public void CreateZipFile(string destDirectory)
        {

            string rootFolderPath = Path.Combine(destDirectory, "ARMB");
            string imagesFolderPath = Path.Combine(rootFolderPath, "images");
            string audioFolderPath = Path.Combine(rootFolderPath, "audio");
            string videoFolderPath = Path.Combine(rootFolderPath, "video");
            string configPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "config.xml");
            string configZipPath = Path.Combine(rootFolderPath, "config.xml");
            string zipPath = Path.Combine(destDirectory, "archive.armb");

            //If there is already a zip file present delete it so a new one can be created.
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            //If the ARMB folder we're about to make into a .zip already exists, delete it. 
            //(It shouldn't, but it does if this function crashes between creating and deleting it.)
            if (Directory.Exists(rootFolderPath))
            {
                Directory.Delete(rootFolderPath, true);
            }

            Directory.CreateDirectory(rootFolderPath);
            Directory.CreateDirectory(imagesFolderPath);
            Directory.CreateDirectory(audioFolderPath);
            Directory.CreateDirectory(videoFolderPath);

            File.Copy(configPath, configZipPath);

            foreach (BB_Page page in Pages)
            {
                if (page.SourcePageImageFileName != null)
                {
                    try
                    {
                        string imageSourcePath = page.SourcePageImageFileName;
                        string imageDestinationPath = Path.Combine(imagesFolderPath, page.PageImageFileName);

                        File.Copy(imageSourcePath, imageDestinationPath);
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                if (page.SourceAudioFileName != null)
                {
                    try
                    {
                        string audioSourcePath = page.SourceAudioFileName;
                        string audioDestinationPath = Path.Combine(audioFolderPath, page.AudioFileName);

                        File.Copy(audioSourcePath, audioDestinationPath);
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                if (page.SourceVideoFileName != null)
                {
                    try
                    {
                        string videoSourcePath = page.SourceVideoFileName;
                        string videoDestinationPath = Path.Combine(videoFolderPath, page.VideoFileName);

                        File.Copy(videoSourcePath, videoDestinationPath);
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            //Only have one copy of config.xml - the one in the zip file
            File.Delete(configPath);

            try
            {
                // Put ARMB folder in zip file, false parameter means do not include the root directory when unzipping
                ZipFile.CreateFromDirectory(rootFolderPath, zipPath, CompressionLevel.NoCompression, false);
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }

            //Recursively (boolean parameter) delete ARMB folder to just leave the zip file
            Directory.Delete(rootFolderPath, true);
        }

        /// <summary>Converts information about the book to a string.</summary>
        /// <return>String of info about the book.</return>
		public override string ToString()
        {
            string bookString = "";
            bookString += "Title: " + Title + "\n";

            foreach (string author in Authors)
            {
                bookString += "Author: " + author + "\n";
            }

            foreach (BB_Page page in Pages)
            {
                bookString += "Page Num: " + page.PageNumber + "\n";
                bookString += "Page Image: " + page.PageImageFileName + "\n";
                if (page.AudioFileName != null)
                {
                    bookString += "Page Audio File " + page.AudioFileName + "\n";
                }
                if (page.VideoFileName != null)
                {
                    bookString += "Page Video File " + page.VideoFileName + "\n";
                    bookString += "Page Video width " + page.VideoWidth + "\n";
                    bookString += "Page Video height " + page.VideoHeight + "\n";
                    bookString += "Page Video xcoord " + page.VideoX + "\n";
                    bookString += "Page Video ycoord " + page.VideoY + "\n";
                }
            }

            return bookString;
        }





        public void AddPage(BB_Page p)
        {
            Pages.Add(p);
        }
    }
}
