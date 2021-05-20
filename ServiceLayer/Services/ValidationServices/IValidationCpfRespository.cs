﻿using DomainLayer.Models.Validations;

namespace ServiceLayer.Services.ValidationServices
{
    public interface IValidationCpfRespository
    {
        bool ValidaCpf(ICpfModel cpfModel);
    }
}