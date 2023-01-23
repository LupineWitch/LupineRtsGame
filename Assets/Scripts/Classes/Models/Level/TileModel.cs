using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml;
using UnityEngine;

namespace Assets.Scripts.Classes.Models.Level
{
    public class TileModel : ILevelPartXMLModel<TileModel>
    {
        public Type TileType { get; set; }
        public Vector3Int Position { get; set; }
        public string AssetName { get; set; }

        public void DeserialiseFromNode(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public XmlNode SerialiseToNode(XmlDocument doc)
        {
            var parentNode = doc.CreateElement(nameof(TileModel));
            var positionNode = doc.CreateElement(nameof(Position));
            var spriteNode = doc.CreateElement(nameof(AssetName));
            parentNode.SetAttribute(nameof(TileType), TileType.FullName);
            positionNode.AppendChild(doc.CreateTextNode(Position.ToString()));
            spriteNode.AppendChild(doc.CreateTextNode(AssetName.ToString()));

            parentNode.AppendChild(spriteNode);
            parentNode.AppendChild(positionNode);
            return parentNode;
        }
    }
}
