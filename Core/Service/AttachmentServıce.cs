﻿using System;
using System.Linq;
using Core.Repository;
using Model;

namespace Core.Service
{

    public interface IAttachmentService
    {
        Attachment GetAttachment(Guid id);
    }


    public class AttachmentServıce: IAttachmentService
    {
        private readonly IRepository<Attachment> _repository; 
        public AttachmentServıce(IRepository<Attachment> repository)
        {
            _repository = repository;
        }

        public Attachment GetAttachment(Guid id)
        {
            var attachment = _repository.Collection.FirstOrDefault(i => i.Id == id);

            return attachment;
        }
    }

    
}
