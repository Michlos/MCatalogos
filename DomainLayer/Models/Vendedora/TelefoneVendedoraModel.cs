﻿using DomainLayer.DataAttribute;
using DomainLayer.Models.CommonModels.Enums;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.Vendedora
{
    public class TelefoneVendedoraModel : ITelefoneVendedoraModel
    {
        [Key]
        public int TelefoneId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "O número do telfone deve ser informado")]
        [StringLength(11)]
        [Index("UK_TelVend", IsUnique = true)]
        [RegularExpression(@"\d{11}", ErrorMessage = "Utilize somente números")]
        public string Numero { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Informe o Tipo de Telefone. É obrigatório.")]
        public int TipoTelefoneId { get; set; }
        public virtual TipoTelefoneModel TipoTelefone { get; set; }

        [Required]
        [CascadeDelete]
        public int VendedoraId { get; set; }

        public virtual VendedoraModel Vendedora { get; set; }

    }
}
