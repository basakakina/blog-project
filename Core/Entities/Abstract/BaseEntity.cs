using ApplicationCore.Entities.Enums;

namespace ApplicationCore.Entities.Abstract
{
    public abstract class BaseEntity
    {
        private DateTime _createdDate = DateTime.Now;
        private Status _status = Status.Active;

        public Guid Id { get; set; }
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public Status Status { get => _status; set => _status = value; }
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";

        
    }
}
