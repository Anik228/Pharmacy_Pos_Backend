namespace pharmacy_pos_system.module.role.model
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Boolean IsDeleted { get; set; }

    }
}
