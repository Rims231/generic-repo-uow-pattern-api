namespace generic_repo_uow_pattern_api.Entity
{
    public class Order
    {

        public int Id { get; set; }  

        public int ProductId { get; set; }  

        public Product Product { get; set; } = null!; 

        public DateTime OrderDate { get; set; }

    }
}
