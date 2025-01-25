namespace pharmacy_pos_system.module.user.model
{
   
      
        public class AddMedicineDto
        {         
            public string Name { get; set; }
            public string Brand { get; set; }
            public string Image { get; set; }
            public string Description { get; set; }

            public int Price { get; set; }

            public int Quantity{ get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;

            public DateTime Expiry_date { get; set; } = DateTime.Now;


    }


}
