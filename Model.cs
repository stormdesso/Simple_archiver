using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lab6_v2
{        
    class Archivator
    {
        public delegate void Notify(int prog);
        public event Notify notifier;

        private static bool _state = false;

        public void Pack(string FileRes, string FileDest)//исходник , конечный
        {
            Progress = 0;
            notifier?.Invoke(Progress);

            FileStream fs = null;
            fs = new FileStream(FileRes, FileMode.Open, FileAccess.Read);
            List<byte> loadData = new List<byte>();   
            
            while (fs.Position < fs.Length)//записываем байты из потока в список
            {                
                byte sign = (byte)fs.ReadByte();                               
                loadData.Add(sign);                
            }            
            fs.Close();
            
            byte numberOfDuplicate = 0;
            byte numberOfDifferentsBytes = 0;
            List<byte> archivedData = new List<byte>();
            List<byte> bufferOfDifferentByte = new List<byte>();
            for (int i = 0; i < loadData.Count - 1; i++)
            {
                if (_state == true)                                    
                    break;
                
                if (loadData[i] == loadData[i + 1])
                {
                    if (numberOfDifferentsBytes > 0)
                    {
                        archivedData.Add(0);
                        archivedData.Add(numberOfDifferentsBytes);
                        archivedData.AddRange(bufferOfDifferentByte);
                        bufferOfDifferentByte.Clear();
                        numberOfDifferentsBytes = 0;
                    }
                    if (numberOfDuplicate == 255)
                    {
                        archivedData.Add(numberOfDuplicate);
                        archivedData.Add(loadData[i]);
                        numberOfDuplicate = 0;
                    }
                    numberOfDuplicate++;
                }
                else 
                {
                    if (numberOfDuplicate > 0)
                    {
                        numberOfDuplicate++;
                        archivedData.Add(numberOfDuplicate);
                        archivedData.Add(loadData[i]);
                        numberOfDuplicate = 0;
                    }
                    else 
                    {
                        if (numberOfDifferentsBytes == 255)
                        {
                            archivedData.Add(0);
                            archivedData.Add(numberOfDifferentsBytes);
                            archivedData.AddRange(bufferOfDifferentByte);
                            bufferOfDifferentByte.Clear();
                            numberOfDifferentsBytes = 0;
                        }
                        bufferOfDifferentByte.Add(loadData[i]);
                        numberOfDifferentsBytes++;
                    }
                }

                if (i == loadData.Count - 2)
                {
                    if (numberOfDuplicate > 0)
                    {
                        numberOfDuplicate++;
                        archivedData.Add(numberOfDuplicate);
                        archivedData.Add(loadData[i]);
                        numberOfDuplicate = 0;
                    }
                    if (numberOfDifferentsBytes > 0)
                    {
                        bufferOfDifferentByte.Add(loadData[i + 1]);
                        archivedData.Add(0);
                        numberOfDifferentsBytes++;
                        archivedData.Add(numberOfDifferentsBytes);
                        archivedData.AddRange(bufferOfDifferentByte);
                        bufferOfDifferentByte.Clear();
                        numberOfDifferentsBytes = 0;
                    }
                }
                
                double pos = i;
                double length = loadData.Count-2;
                Progress = (int)(pos/length * 100);
                notifier?.Invoke(Progress);               
            }

            if (_state == false)
            {
                FileStream fw = new FileStream(FileDest, FileMode.Create);
                Byte[] temp = new Byte[archivedData.Count];
                archivedData.CopyTo(temp);
                fw.Write(temp, 0, temp.Length);
                fw.Close();
            }
            _state = false;
        }
            
        public void Unpack(string FileRes, string FolderDest)
        {
            Progress = 0;
            notifier?.Invoke(Progress);

            FileStream fs  = new FileStream(FileRes, FileMode.Open);
            List<byte> loadData = new List<byte>();

            while (fs.Position < fs.Length)//записываем байты из потока в список
            {
                byte sign = (byte)fs.ReadByte();
                loadData.Add(sign);
            }
            fs.Close();

            List<byte> extractData = new List<byte>();

            for (int i = 0; i < loadData.Count - 1; i++)
            {
                if (_state == true)
                    break;

                if (loadData[i] == 0)
                {
                    i++;
                    byte sequenceLength = loadData[i];
                    for (int j = 0; j < sequenceLength; j++)
                    {
                        i++;
                        extractData.Add(loadData[i]);
                    }
                }
                else 
                {
                    byte sequenceLength = loadData[i];
                    i++;
                    for (int j = 0; j < sequenceLength; j++)                 
                        extractData.Add(loadData[i]);                    
                }

                double pos = i;
                double length = loadData.Count-2;
                Progress = (int)(pos / length * 100);
                notifier?.Invoke(Progress);
            }

            if (_state == false)
            {
                FileStream fw = new FileStream(FolderDest, FileMode.Create);
                Byte[] temp = new Byte[extractData.Count];
                extractData.CopyTo(temp);
                fw.Write(temp, 0, temp.Length);
                fw.Close();
            }
            _state = false;
        }

        public static void Stop(bool state)
        {
            _state = state;            
        }
        
        public static int Progress { get; private set; }

    }    
}
