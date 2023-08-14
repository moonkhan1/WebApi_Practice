namespace WebApi_Practice.Data.Entities
{
    public class BaseEntity<TPrimaryKey>
    {
        public virtual TPrimaryKey Id {get; set;}
    }
}
