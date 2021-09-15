using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using canonapi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace canonapi.configuration
{
    public class AnnotationService : IAnnotationService
    {
        private readonly IMongoCollection<AnnotationObject> _annotation;
        private readonly AnnotationConfiguration _settings;
        public AnnotationService(IOptions<AnnotationConfiguration> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);
            _annotation = database.GetCollection<AnnotationObject>(_settings.AnnotationCollectionName);
        }
        public List<AnnotationObject> GetAll()
        {
            return _annotation.Find(c => true).ToList();
        }
        public AnnotationObject GetById(string id)
        {
            return _annotation.Find<AnnotationObject>(c => c.id == id).FirstOrDefault();
        }
        public AnnotationObject Create(AnnotationObject book)
        {
            _annotation.InsertOne(book);
            return book;
        }
        public void Update(string id, AnnotationObject book)
        {
            _annotation.ReplaceOne(c => c.id == id, book);
        }
        public void Delete(string id)
        {
            _annotation.DeleteOne(c => c.id == id);
        }
    }
}
