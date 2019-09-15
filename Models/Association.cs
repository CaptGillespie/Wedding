using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
    
public class Association
{
    [Key]
    public int AssociationId {get;set;}
    public int WeddingModelId {get;set;}
    public int RegisterUserId {get;set;}

    public RegisterUser Creator {get;set;}
    public WeddingModel wedding {get;set;}

}