using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Assets.Scripts.Classes.Models.Level
{
    public abstract class TileObjectModel : ILevelPartXMLModel<TileObjectModel>
    {
        public string Name { get; set; }
        public Type TileObjectType { get; set; }
        public Vector3 Position { get; set; }

        public virtual XmlNode SerialiseToNode(XmlDocument document)
        {
            var parentNode = document.CreateElement(nameof(TileObjectModel));
            parentNode.SetAttribute(nameof(Type), TileObjectType.FullName);
            var nameNode = document.CreateElement(nameof(Name));
            nameNode.AppendChild(document.CreateTextNode(Name));
            var positionNode = document.CreateElement(nameof(Position));
            positionNode.AppendChild(document.CreateTextNode(Position.ToString()));

            parentNode.AppendChild(nameNode);
            parentNode.AppendChild(positionNode);
            return parentNode;
        }

        public virtual void DeserialiseFromNode(XmlNode node)
        {
            throw new NotImplementedException();
        }
    }

    public class TileScriptModel : TileObjectModel
    {

    }
    
    public class TileGameObjectModel : TileObjectModel
    {

    }
}
