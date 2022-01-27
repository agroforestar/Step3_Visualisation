using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Parseur
{
    public class Parseur
    {
        // Start is called before the first frame update
        protected FileInfo theSourceFile = null;
        protected StreamReader reader = null;
        protected string text = " "; // assigned to allow first line to be read below

        void Start()
        {

        }

        public Dictionary<string, List<Vector3>> readFile(string path)
        {
            theSourceFile = new FileInfo(path);
            reader = theSourceFile.OpenText();
            Dictionary<string, List<Vector3>> dictionary = new Dictionary<string, List<Vector3>>();
            text = reader.ReadLine();
            do
            {
                string[] element = text.Split(';');
                string form = element[0];
                string color = element[1];
                int x = Int32.Parse(element[2]);
                int y = Int32.Parse(element[3]);
                AddElement(form, new Vector3(x, 0, y), dictionary);
                
                text = reader.ReadLine();
            } while (text != null);
            return dictionary;
        }

        void AddElement(string type, Vector3 coord, Dictionary<string, List<Vector3>> all)
        {
            if (all.ContainsKey(type))
            {
                all[type].Add(coord);
            }
            else
            {
                all.Add(type,new List<Vector3> { coord });
            }
        }

        void Update()
        {

        }
    }
}
