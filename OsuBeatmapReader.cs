using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace OsuBeatmapReader
{
    public class OsuBeatmapReader
    {
        /// <summary>
        /// All Data, as full String
        /// </summary>
        public string RAW_Data;

        public int OsuFileFormat;
        public string BackgroundFileName, BackgroundFilePath, AudioFilePath;

        /// <summary>
        /// File Read Error Message
        /// </summary>
        public string Error;

        #region General

        public string AudioFileName;
        public int AudioPreviewTime;

        /// <summary>
        /// GameMode: 1=osu , 2=taiko , 3=catch , 4=mania
        /// </summary>
        public int GameMode;

        #endregion General

        #region Metadata

        public string Title;
        public string TitleUnicode;
        public string Artist;
        public string ArtistUnicde;
        public string Creator;
        public string Version;
        public string Source;
        public string Tags;
        public int BeatmapID;
        public int BeatmapSetID;

        #endregion Metadata

        #region Difficulty

        public float HP;
        public float CS;
        public float OD;
        public float AR;
        public float SliderMultiplier;
        public float SliderTickrate;

        #endregion Difficulty
        private bool gotBG = false;
        public List<int> Circle_PosX;
        public List<int> Circle_PosY;
        public List<int> Circle_Time;

        public List<int> TimingPoint_Time;
        public List<int> TimingPoint_Meter;
        public List<float> TimingPoint_Bpm;
        public List<int> InheritedPoint_Time;
        public List<float> InheritedPoint_Multiplier;

        public OsuBeatmapReader()
        {
            Circle_PosX = new List<Int32>();
            Circle_PosY = new List<Int32>();
            Circle_Time = new List<Int32>();

            TimingPoint_Time = new List<Int32>();
            TimingPoint_Meter = new List<Int32>();
            TimingPoint_Bpm = new List<float>();
            InheritedPoint_Time = new List<Int32>();
            InheritedPoint_Multiplier = new List<float>();
        }

        // Sliders are not supported yet.

        ///<summary>
        /// Get osu! Beatmap Data (string: .osu path)
        ///</summary>
        public void GetBeatmapData(string path)
        {
            #region ClearListCheck

            if (Circle_PosX == null)
            {
            }
            else
            {
                Circle_PosX.Clear();
            }

            if (Circle_PosY == null)
            {
            }
            else
            {
                Circle_PosY.Clear();
            }

            if (Circle_Time == null)
            {
            }
            else
            {
                Circle_Time.Clear();
            }

            if (TimingPoint_Time == null)
            {
            }
            else
            {
                TimingPoint_Time.Clear();
            }
            if (TimingPoint_Meter == null)
            {
            }
            else
            {
                TimingPoint_Meter.Clear();
            }

            if (TimingPoint_Bpm == null)
            {
            }
            else
            {
                TimingPoint_Bpm.Clear();
            }
            if (InheritedPoint_Time == null)
            {
            }
            else
            {
                InheritedPoint_Time.Clear();
            }
            if (InheritedPoint_Multiplier == null)
            {
            }
            else
            {
                InheritedPoint_Multiplier.Clear();
            }

            #endregion ClearListCheck

            BackgroundFileName = "";
            BackgroundFilePath = "";
            RAW_Data = "";
            OsuFileFormat = -1;
            AudioFileName = "";
            AudioFilePath = "";
            AudioPreviewTime = -1;
            GameMode = -1;
            Title = "";
            TitleUnicode = "";
            Artist = "";
            ArtistUnicde = "";
            Creator = "";
            Version = "";
            Source = "";
            Tags = "";
            BeatmapID = -1;
            BeatmapSetID = -1;
            HP = -1;
            CS = -1;
            OD = -1;
            AR = -1;
            SliderMultiplier = -1;
            SliderTickrate = -1;

            string bmpath = path;

            FileStream fs = new FileStream(bmpath, FileMode.Open);
            StreamReader streamReader = new StreamReader(fs);

            bool GeneralTag = false;
            bool MetaDataTag = false;
            bool DifficultyTag = false;
            bool EventsTag = false;
            bool TimingPointsTag = false;
            bool HitObjectsTag = false;
            bool ColoursTag = false;
            Console.WriteLine("Reading Started.");
            // try {
            while (streamReader.Peek() != -1)
            {
                string str = streamReader.ReadLine();
                Console.WriteLine(str);
                if (str.StartsWith("//") || str.Equals(""))
                {
                    Console.WriteLine("Command found: " + str);
                    //Skip Line
                }
                else
                {
                    if ((GeneralTag == false) && (MetaDataTag == false) && (DifficultyTag == false) && (EventsTag == false) && (TimingPointsTag == false) && (HitObjectsTag == false))
                    {
                        if (str.StartsWith("osu file format v"))
                        {
                            OsuFileFormat = Convert.ToInt32(str.Replace("osu file format v", ""));
                            Console.WriteLine("osu! file format v" + OsuFileFormat);
                        }
                    }

                    if (str.Equals("[General]"))
                    {
                        GeneralTag = true;
                        Console.WriteLine("General Section");
                    }
                    if (str.Equals("[Metadata]"))
                    {
                        GeneralTag = false;
                        MetaDataTag = true;
                        Console.WriteLine("Metadata Section");
                    }
                    if (str.Equals("[Difficulty]"))
                    {
                        Console.WriteLine("Difficulty Section");
                        MetaDataTag = false;
                        DifficultyTag = true;
                    }

                    if (str.Equals("[Events]"))
                    {
                        Console.WriteLine("Event Section");
                        DifficultyTag = false;
                        EventsTag = true;
                    }

                    if (str.Equals("[TimingPoints]"))
                    {
                        Console.WriteLine("TimingPoints Section");
                        EventsTag = false;
                        TimingPointsTag = true;
                        
                    }

                    if (str.Equals("[Colours]"))
                    {
                        Console.WriteLine("Colours Section");
                        TimingPointsTag = false;
                        ColoursTag = true;

                    }


                    if (str.Equals("[HitObjects]"))
                    {
                        Console.WriteLine("HitObjects Section");
                        ColoursTag = false;
                        TimingPointsTag = false;
                        HitObjectsTag = true;
                    }
                    

                   

                  
                    if (HitObjectsTag)
                    {
                        if (str.Equals("[HitObjects]"))
                        {
                        }
                        else
                        {

                        

                            if (str.Contains("|"))
                            {
                                //READING SLIDERS AS CIRCLES FOR NOW
                                string[] strArray = str.Split(',');
                                for (int index = 0; index < strArray.Length; ++index)
                                {
                                    if (index == 0)
                                    {
                                        Circle_PosX.Add(Convert.ToInt32(strArray[index]));
                                    }

                                    if (index == 1)
                                    {
                                        Circle_PosY.Add(Convert.ToInt32(strArray[index]));
                                    }
                                    if (index == 2)
                                    {
                                        Circle_Time.Add(Convert.ToInt32(strArray[index]));
                                    }
                                }
                            }
                            else
                            {
                                string[] strArray = str.Split(',');
                                for (int index = 0; index < strArray.Length; ++index)
                                {
                                    if (index == 0)
                                    {
                                        Circle_PosX.Add(Convert.ToInt32(strArray[index]));
                                    }

                                    if (index == 1)
                                    {
                                        Circle_PosY.Add(Convert.ToInt32(strArray[index]));
                                    }
                                    if (index == 2)
                                    {
                                        Circle_Time.Add(Convert.ToInt32(strArray[index]));
                                    }
                                }
                            }
                        }
                    }

                    if (ColoursTag)
                    {
                        //IGNORE
                    }

                    if (TimingPointsTag)
                    {
                        if (str.Equals("[TimingPoints]"))
                        {

                        }
                        else
                        {


                            string[] strArray = str.Split(',');
                            for (int index = 0; index < strArray.Length; ++index)
                            {
                                int Time = 0, Uninherited = 0, Meter=4;
                                float BeatLength = 0;
                                if (index == 0)
                                {



                                    Time = int.Parse(strArray[index]);
                                }
                                if (index == 1)
                                {
                                    BeatLength = Convert.ToSingle(strArray[index]);
                                }
                                if (index == 2)
                                {
                                    Meter = int.Parse(strArray[index]);
                                }

                                if (index == 6)
                                {
                                    Uninherited = Convert.ToInt32(strArray[index]);
                                }

                                if (Uninherited == 0)
                                {
                                    Console.WriteLine(Time);
                                    TimingPoint_Time.Add(Time);
                                    TimingPoint_Meter.Add(Meter);
                                    TimingPoint_Bpm.Add(((BeatLength * 1000) * 60) / 1);
                                }
                                else
                                {
                                    if (Uninherited == 1)
                                    {
                                        InheritedPoint_Time.Add(Time);
                                        InheritedPoint_Multiplier.Add(100 / (BeatLength * -1));
                                    }
                                }
                            }
                        }
                    }

                    if (EventsTag)
                    {

                        string[] strArray = str.Split(',');
                        if (gotBG == false)
                        {
                            if (str.StartsWith("0,0,\""))
                            {

                                string temp;
                                temp = str.Replace("0,0,\"", "");

                                if (temp.Contains("\",0,0")){
                                    temp = temp.Replace("\",0,0", "");
                                }




                                if(File.Exists(Path.GetDirectoryName(bmpath) + "\\" + temp))
                                {
                                    BackgroundFilePath = Path.GetDirectoryName(bmpath) + "\\" + temp;
                                    BackgroundFileName = temp;
                                    Console.WriteLine("BG: " + BackgroundFileName);
                                    gotBG = true;
                                }
                                else
                                {
                               
                                }
                               
                              


                               
                              
                               
                                    
                                
                            }
                        }
                        
                    }
                    if (DifficultyTag)
                    {
                        if (str.StartsWith("HPDrainRate:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            HP = Convert.ToSingle(str.Replace("HPDrainRate:", ""));
                            Console.WriteLine("HP: " + HP);
                        }
                        if (str.StartsWith("CircleSize:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            CS = Convert.ToSingle(str.Replace("CircleSize:", ""));
                            Console.WriteLine("CS: " + CS);
                        }
                        if (str.StartsWith("OverallDifficulty:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            OD = Convert.ToSingle(str.Replace("OverallDifficulty:", ""));
                            Console.WriteLine("OD: " + OD);
                        }
                        if (str.StartsWith("ApproachRate:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            AR = Convert.ToSingle(str.Replace("ApproachRate:", ""));
                            Console.WriteLine("AR: " + AR);
                        }
                        if (str.StartsWith("SliderMultiplier:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            SliderMultiplier = Convert.ToSingle(str.Replace("SliderMultiplier:", ""));
                            Console.WriteLine("SliderMultiplier: " + SliderMultiplier);
                        }
                        if (str.StartsWith("SliderTickRate:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            SliderTickrate = Convert.ToSingle(str.Replace("SliderTickRate:", ""));
                            Console.WriteLine("SliderTickRate: " + SliderTickrate);
                        }
                    }

                    if (MetaDataTag)
                    {
                        if (str.StartsWith("Title:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            Title = Convert.ToString(str.Replace("Title:", ""));
                            Console.WriteLine("Title: " + Title);
                        }
                        if (str.StartsWith("TitleUnicode:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            TitleUnicode = Convert.ToString(str.Replace("TitleUnicode:", ""));
                            Console.WriteLine("TitleU: " + TitleUnicode);
                        }
                        if (str.StartsWith("Artist:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            Artist = Convert.ToString(str.Replace("Artist:", ""));
                            Console.WriteLine("Artist: " + Artist);
                        }
                        if (str.StartsWith("ArtistUnicde:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            ArtistUnicde = Convert.ToString(str.Replace("ArtistUnicde:", ""));
                            Console.WriteLine("ArtistU: " + ArtistUnicde);
                        }
                        if (str.StartsWith("Creator:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            Creator = Convert.ToString(str.Replace("Creator:", ""));
                            Console.WriteLine("Creator: " + Creator);
                        }
                        if (str.StartsWith("Version:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            Version = Convert.ToString(str.Replace("Version:", ""));
                            Console.WriteLine("Version: " + Version);
                        }
                        if (str.StartsWith("Source:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            Source = Convert.ToString(str.Replace("Source:", ""));
                            Console.WriteLine("Source: " + Source);
                        }
                        if (str.StartsWith("Tags:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            Tags = Convert.ToString(str.Replace("Tags:", ""));
                            Console.WriteLine("Tags: " + Tags);
                        }
                        if (str.StartsWith("BeatmapID:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            BeatmapID = Convert.ToInt32(str.Replace("BeatmapID:", ""));
                            Console.WriteLine("BeatmapID: " + BeatmapID);
                        }
                        if (str.StartsWith("BeatmapSetID:"))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            BeatmapSetID = Convert.ToInt32(str.Replace("BeatmapSetID:", ""));
                            Console.WriteLine("BeatMapSetID: " + BeatmapSetID);
                        }
                    }
                    if (GeneralTag)
                    {
                        if (str.StartsWith("AudioFilename: "))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            AudioFileName = Convert.ToString(str.Replace("AudioFilename: ", ""));
                            AudioFilePath = bmpath + "\\" + AudioFileName;
                            Console.WriteLine("Audio Filename: " + AudioFileName + Environment.NewLine + AudioFilePath);
                        }

                        if (str.StartsWith("PreviewTime: "))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            AudioPreviewTime = Convert.ToInt32(str.Replace("PreviewTime: ", ""));
                            Console.WriteLine("PreviewTime: " + AudioPreviewTime);
                        }

                        if (str.StartsWith("Mode: "))
                        {
                            RAW_Data = RAW_Data + str + Environment.NewLine;
                            GameMode = Convert.ToInt32(str.Replace("Mode:", ""));
                            Console.WriteLine("GameMode: " + GameMode);
                        }
                    }
                    

                }
            }
            streamReader.Close();
            fs.Close();
            if (GameMode == -1)
            {
                GameMode = 0;
            }
            /*}
            catch (Exception ex)
            {
                Error = ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.Source;
            }*/
        }
    }
}