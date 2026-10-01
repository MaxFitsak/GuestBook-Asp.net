using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebGuestBook.Models;

namespace WebGuestBook.Models.Review;

    public class Review
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Введіть текст повідомлення")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Повідомлення має бути від 2 до 1000 символів")]
        [Display(Name = "Повідомлення")]
        public required string Message { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Ім'я автора")]
        public required string Author { get; set; }
        
        [Display(Name = "Дата відгуку")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User.User? User { get; set; }
    }
