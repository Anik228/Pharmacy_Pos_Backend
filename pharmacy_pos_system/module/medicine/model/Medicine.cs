using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace pharmacy_pos_system.module.medicine.model
{
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime Expiry_date { get; set; } = DateTime.UtcNow;

    }

}
