using NPOI.XWPF.UserModel;

namespace generic_repo_uow_pattern_api.Entity
{
    public class Product
    {

       
            public int Id { get; set; }  // ✅ Primary Key

            public string ProductName { get; set; } = string.Empty;

            public decimal Price { get; set; }

           
            public List<Order> Orders { get; set; } = new List<Order>();
        
    }
}
