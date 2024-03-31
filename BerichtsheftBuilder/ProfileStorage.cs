﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Forms;
using BerichtsheftBuilder.dto;
using System.Collections.Generic;

namespace BerichtsheftBuilder
{
    public class ProfileStorage
    {
        private bool created;
        public bool Created
        {
            get => created;
            set => created = value;
        }

        private string name;
        public string Name
        {
            get => name;
            set => name = value;
        }

        private string ausbilderName;
        public string AusbilderName
        {
            get => ausbilderName;
            set => ausbilderName = value;
        }

        private DateTime ausbildungsstart;
        public DateTime Ausbildungsstart
        {
            get => ausbildungsstart;
            set => ausbildungsstart = value;
        }

        private DateTime ausbildungsend;
        public DateTime Ausbildungsend
        {
            get => ausbildungsend;
            set => ausbildungsend = value;
        }

        private string ausbildungsabteilung;
        public string Ausbildungsabteilung
        {
            get => ausbildungsabteilung;
            set => ausbildungsabteilung = value;
        }

        private List<TaskDTO> taskList;
        public List<TaskDTO> TaskList
        {
            get => taskList;
            set => taskList = value;
        }

        public delegate void OnReadDelegate();
        private OnReadDelegate onRead;
        public OnReadDelegate OnRead
        {
            get => onRead;
            set => onRead = value;
        }

        public ProfileStorage()
        {
            created = false;
            name = "";
            ausbilderName = "";
            ausbildungsstart = new DateTime();
            ausbildungsend = new DateTime();
            taskList = new List<TaskDTO>();
            onRead = new OnReadDelegate(() => { });
        }

        private void BinaryWriter(IFormatter formatter, FileStream stream, object obj)
        {
            if (formatter == null || stream == null)
            {
                return;
            }

            try
            {
                formatter.Serialize(stream, obj);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private object BinaryRead(IFormatter formatter, FileStream stream)
        {
            if (formatter == null || stream == null)
            {
                return null;
            }

            return formatter.Deserialize(stream);
        }

        public bool Save()
        {
            try
            {
                FileStream stream = File.Open("profile.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                if (stream == null)
                {
                    return false;
                }

                if (!stream.CanWrite)
                {
                    stream.Close();
                    return false;
                }

                IFormatter formatter = new BinaryFormatter();

                BinaryWriter(formatter, stream, created);
                BinaryWriter(formatter, stream, name);
                BinaryWriter(formatter, stream, ausbilderName);
                BinaryWriter(formatter, stream, ausbildungsstart);
                BinaryWriter(formatter, stream, ausbildungsend);
                BinaryWriter(formatter, stream, ausbildungsabteilung);
                BinaryWriter(formatter, stream, taskList);

                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        public bool Read()
        {
            try
            {
                FileStream stream = File.Open("profile.bin", FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                if (stream == null)
                {
                    return false;
                }

                if (!stream.CanRead)
                {
                    stream.Close();
                    return false;
                }

                if (stream.Length == 0)
                {
                    stream.Close();
                    return false;
                }

                IFormatter formatter = new BinaryFormatter();

                created = (bool)BinaryRead(formatter, stream);
                name = (string)BinaryRead(formatter, stream);
                ausbilderName = (string)BinaryRead(formatter, stream);
                ausbildungsstart = (DateTime)BinaryRead(formatter, stream);
                ausbildungsend = (DateTime)BinaryRead(formatter, stream);
                ausbildungsabteilung = (string)BinaryRead(formatter, stream);
                taskList = (List<TaskDTO>)BinaryRead(formatter, stream);

                stream.Close();
                onRead.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }
    }
}
