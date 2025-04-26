using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Patrimonios.Data;

public class PatrimoniosUser : IdentityUser
{
    public string Name {  get; set; }

    [Required(ErrorMessage = "Grupo é obrigatório.")]
    public int Grupo {  get; set; }
}

