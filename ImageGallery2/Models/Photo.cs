using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Models
{
    [Table("PHOTO", Schema = "dbo")]
    public class Photo
    {
        [Key, Column("ID"), DatabaseGenerated(DatabaseGeneratedOption.Identity), Display(Name = "Ид")]
        public int Id { get; set; }

        [Column("KIND"), Display(Name = "Сущность")]
        public Int16 Kind { get; set; }

        [Column("KIND_ID"), Display(Name = "Ид сущности")]
        public int KindId { get; set; }

        [StringLength(255), Column("NAME"), Display(Name = "Имя")]
        public string Name { get; set; }

        [StringLength(255), Column("MEMO"), Display(Name = "Примечание"), DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [StringLength(20), Column("EXTENTION"), DatabaseGenerated(DatabaseGeneratedOption.Computed), Display(Name = "Расширение")]
        public string Extension { get; private set; }

        [Column("CONTENT"), Display(Name = "Изображение")]
        public byte[] Content { get; set; }
    }
}
