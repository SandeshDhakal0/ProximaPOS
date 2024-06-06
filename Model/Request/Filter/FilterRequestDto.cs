﻿using System.Runtime.InteropServices.JavaScript;

namespace TheHighInnovation.POS.WEB.Model.Request.Filter;

public class FilterRequestDto
{
	public int OrganizationId { get; set; }

	public int CompanyId { get; set; }

	public int? CategoryId { get; set; }


	public int PageSize { get; set; }

	public int PageCount { get; set; }

	public string Search { get; set; } = "";
    
	public bool IsInitialized { get; set; }

	public string StartDate { get; set; }

	public string EndDate { get; set; }
	
	public string TransactionOption { get; set; }	
}

public class VendorFilter
{
	public string VendorName { get; set; } = string.Empty;
	public string PanVat { get; set; } = string.Empty;
	public bool IsActive { get; set; }
	public int PageNo { get; set; } = 1;
	public int PageSize { get; set; } = 6;
    public bool IsInitialized { get; set; }
}

public class CategoryFilter
{
	public string CategoryCode { get; set; } = string.Empty;
	public string CategoryName { get; set; } = string.Empty;
	public int PageNo { get; set; }
	public int PageSize { get; set; } = 10;
    public bool IsInitialized { get; set; }

}


public class InventoryRecordsFilter
{
    public int? VendorId { get; set; } 
	public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 6;
    public bool IsInitialized { get; set; }
}

public class ProductServiceFilter
{
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 10;
	public bool IsInitialized { get; set; } = true;
}

public class KharidKhataFilter
{
	public int YearBs { get; set; } = 2081;
	public int MonthBs { get; set; } = 0;
	public int PageNo { get; set; } = 1;
	public int PageSize { get; set; } = 5;
	public bool IsInitialized { get; set; } =true;
}

