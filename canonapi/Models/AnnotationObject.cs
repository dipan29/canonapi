using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace canonapi.Models
{
    public class AnnotationObject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public List<Marker> markers { get; set; }
    }

    public class VisualTransformMatrix
    {
        public int a { get; set; }
        public int b { get; set; }
        public int c { get; set; }
        public int d { get; set; }
        public int e { get; set; }
        public int f { get; set; }
    }

    public class ContainerTransformMatrix
    {
        public int a { get; set; }
        public int b { get; set; }
        public int c { get; set; }
        public int d { get; set; }
        public int e { get; set; }
        public int f { get; set; }
    }

    public class Marker
    {
        public string drawingImgUrl { get; set; }
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int rotationAngle { get; set; }
        public VisualTransformMatrix visualTransformMatrix { get; set; }
        public ContainerTransformMatrix containerTransformMatrix { get; set; }
        public string typeName { get; set; }
        public string state { get; set; }
    }
}
