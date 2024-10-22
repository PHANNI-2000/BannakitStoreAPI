﻿namespace BannakitStoreApi.Models.Dto
{
    public class ProductUpdateDTO
    {
        public int ProdId { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public string ProdNameTh { get; set; }
        public string ProdNameEn { get; set; }
        public decimal Price { get; set; }
        public decimal? Costprice { get; set; } // ? = nullable is null
        public bool ActiveStatus { get; set; }
        public string? DescTh { get; set; }
        public string? DescEn { get; set; }
        public int? Quatity { get; set; }
        public int? Available { get; set; }
        public int? Tax { get; set; }
        public string? Remark { get; set; }
    }
}
