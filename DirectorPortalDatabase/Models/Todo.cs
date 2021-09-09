using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DirectorPortalDatabase.Models
{
    public class Todo
    {
        /// <summary>
        /// The Primary Key of the address in the database.
        /// Autoincrements.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The title of the todo item
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The description of the todo item
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Whether or not the item is marked as completed
        /// </summary>
        public bool MarkedAsDone { get; set; }
    }
}
