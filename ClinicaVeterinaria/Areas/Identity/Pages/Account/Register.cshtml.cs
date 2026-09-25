using System.ComponentModel.DataAnnotations;
using ClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicaVeterinaria.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
            = new List<AuthenticationScheme>();


        public class InputModel
        {
            [Required(ErrorMessage = "El nombre completo es obligatorio.")]
            [Display(Name = "Nombre completo")]
            public string NombreCompleto { get; set; } = "";


            [Required(ErrorMessage = "El correo es obligatorio.")]
            [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; } = "";


            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(
                100,
                ErrorMessage = "La contraseña debe tener al menos {2} caracteres.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = "";


            [Required(ErrorMessage = "Confirme la contraseña.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare(
                "Password",
                ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; } = "";
        }


        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            ExternalLogins =
                (await _signInManager
                    .GetExternalAuthenticationSchemesAsync())
                    .ToList();
        }


        public async Task<IActionResult> OnPostAsync(
            string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ReturnUrl = returnUrl;

            ExternalLogins =
                (await _signInManager
                    .GetExternalAuthenticationSchemesAsync())
                    .ToList();


            if (!ModelState.IsValid)
            {
                return Page();
            }


            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                NombreCompleto = Input.NombreCompleto,
                EmailConfirmed = true
            };


            var resultado =
                await _userManager.CreateAsync(
                    user,
                    Input.Password
                );


            if (resultado.Succeeded)
            {
                // =====================================
                // ASIGNAR AUTOMÁTICAMENTE ROL CLIENTE
                // =====================================

                var resultadoRol =
                    await _userManager.AddToRoleAsync(
                        user,
                        "Cliente"
                    );


                if (!resultadoRol.Succeeded)
                {
                    foreach (var error in resultadoRol.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description
                        );
                    }

                    return Page();
                }


                // =====================================
                // INICIAR SESIÓN AUTOMÁTICAMENTE
                // =====================================

                await _signInManager.SignInAsync(
                    user,
                    isPersistent: false
                );


                return LocalRedirect(returnUrl);
            }


            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description
                );
            }


            return Page();
        }
    }
}