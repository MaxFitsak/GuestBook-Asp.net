using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGuestBook.Models.User;

public class User
{
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Ім'я повинно містити від 2 до 50 символів.")]
    [Display(Name = "Ім'я")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Прізвище повинно містити від 2 до 50 символів.")]
    [Display(Name = "Прізвище")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    [EmailAddress(ErrorMessage = "Некоректний формат електронної пошти.")]
    [Display(Name = "Електронна пошта")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Поле є обов'язковим для заповнення.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль повинен містити від 6 до 100 символів.")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }
    
}