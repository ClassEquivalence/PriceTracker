namespace PriceTracker.Modules.Repository.Entities
{
    /*
     TODO: Можно унифицировать установку навигационных свойств наследников через какой-нибудь метод, 
    реализовав это дело по типу BaseEntity<NavProps>{ public abstract SetNavProps(NavProps props);}
    и наследуя и реализуя везде NavProps класс. Правда это может быть нецелесообразно.
     */
    public class BaseEntity(int Id)
    {
        public int Id { get; set; } = Id;
    }
}
