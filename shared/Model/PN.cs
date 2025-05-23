namespace shared.Model;

public class PN : Ordination {
	public double antalEnheder { get; set; }
    public List<Dato> dates { get; set; } = new List<Dato>();

    public PN (DateTime startDen, DateTime slutDen, double antalEnheder, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
		this.antalEnheder = antalEnheder;
	}

    public PN() : base(null!, new DateTime(), new DateTime()) {
    }

    /// <summary>
    /// Registrerer at der er givet en dosis på dagen givesDen
    /// Returnerer true hvis givesDen er inden for ordinationens gyldighedsperiode og datoen huskes
    /// Returner false ellers og datoen givesDen ignoreres
    /// </summary>
    
    public bool givDosis(Dato givesDen) {
	    if (givesDen.dato >= startDen && givesDen.dato <= slutDen) {
		    dates.Add(givesDen);
		    return true;
	    } else
	    {
		    return false; 
	    }
    }

    public override double doegnDosis()
    {
	    if (dates.Count == 0) return 0;
	    
	    DateTime første = dates.Min(d => d.dato);
	    DateTime sidste = dates.Max(d => d.dato);
	    int antalDage = (sidste - første).Days + 1;
        
	    return (getAntalGangeGivet() * antalEnheder)/antalDage;
    }


    public override double samletDosis() {
        return dates.Count() * antalEnheder;
    }

    public int getAntalGangeGivet() {
        return dates.Count();
    }

	public override String getType() {
		return "PN";
	}
}

