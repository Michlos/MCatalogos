﻿using DomainLayer.Models.Vendedora;

using ServiceLayer.CommonServices;

using System.Collections.Generic;

namespace ServiceLayer.Services.VendedoraServices
{
    public class VendedoraServices : IVendedoraService, IVendedoraRepository
    {
        private IVendedoraRepository _vendedoraRepository;
        private IModelDataAnnotationCheck _modelDataAnnotationCheck;

        public VendedoraServices(IVendedoraRepository vendedoraRepository, IModelDataAnnotationCheck modelDataAnnotationCheck)
        {
            _vendedoraRepository = vendedoraRepository;
            _modelDataAnnotationCheck = modelDataAnnotationCheck;
        }

        public VendedoraModel Add(IVendedoraModel vendedoraModel)
        {
            return _vendedoraRepository.Add(vendedoraModel);
        }
        public void Update(IVendedoraModel vendedoraModel)
        {
            _vendedoraRepository.Update(vendedoraModel);
        }

        public void Delete(IVendedoraModel vendedoraModel)
        {
            _vendedoraRepository.Delete(vendedoraModel);
        }

        public IEnumerable<IVendedoraModel> GetAll()
        {
            return _vendedoraRepository.GetAll();
        }

        public VendedoraModel GetById(int id)
        {
            return _vendedoraRepository.GetById(id);
        }

        public VendedoraModel GetByCpf(string cpf)
        {
            return _vendedoraRepository.GetByCpf(cpf);
        }
        public void AlteraRotaLetra(int vendedoraId, int rotaLetraId)
        {
            _vendedoraRepository.AlteraRotaLetra(vendedoraId, rotaLetraId);
        }


        public void ValidateModelDataAnnotations(IVendedoraModel vendedoraModel)
        {
            _modelDataAnnotationCheck.ValidateModelDataAnnotations(vendedoraModel);
            
        }

        public void ValidateModel(IVendedoraModel vendedoraModel)
        {
            _modelDataAnnotationCheck.ValidateModelDataAnnotations(vendedoraModel);

        }

    }
}
