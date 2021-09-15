using canonapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace canonapi.configuration
{
    public interface IAnnotationService
    {
        List<AnnotationObject> GetAll();
        AnnotationObject GetById(string id);
        AnnotationObject Create(AnnotationObject obj);
        void Update(string id, AnnotationObject obj);
        void Delete(string id);
    }
}
