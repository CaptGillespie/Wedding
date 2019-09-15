using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
    
public class WeddingModel
{
    [Key]
    public int WeddingId {get;set;}

    [Required]
    public string WedderOne {get;set;}

    [Required]
    public string WedderTwo {get;set;}

    [Required]
    public string Address {get;set;}
    
    [Required]
    public DateTime Date {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;

    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public int RegisterUserId { get; set; }
    // These are Navigation Properties (doesnt actually get added to the DB)
    // You need to use an Include statement to populate this
    public List<Association> Guests { get; set; }
    public RegisterUser Creator { get; set; }





}