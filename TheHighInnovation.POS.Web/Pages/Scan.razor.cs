using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using TheHighInnovation.POS.Model.Request.Scan;
using TheHighInnovation.POS.Model.Response.Base;
using TheHighInnovation.POS.Model.Response.Scan;

namespace TheHighInnovation.POS.Web.Pages;


	public partial class Scan
	{
	private List<ScanResponseDto> _scan { get; set; }
	private bool _showUpsertScanDialog { get; set; }

	private bool _showDeleteScanDialog { get; set; }
	private ScanRequestDto _scanModel { get; set; } = new();

	private string _dialogTitle { get; set; }

	private string _dialogOkLabel { get; set; }

	private string _upsertScanErrorMessage { get; set; }

	private string _deleteScanErrorMessage { get; set; }

protected override async Task OnInitializedAsync()
{
	var result = await BaseService.GetAsync<Derived<List<ScanResponseDto>>>("GetScannedProducts");

	_scan = result?.Result ?? [];
}
private async Task OpenUpsertScanDialog(int? scanId = null)
{
	_dialogTitle = "Add a new Product";

	_dialogOkLabel = "Add";

	_upsertScanErrorMessage = "";

	

	if (scanId.HasValue)
	{
		var parameters = new Dictionary<string, string>
			{
				{ "scanid", scanId.Value.ToString() },
			};

		var result = (await BaseService.GetAsync<Derived<ScanResponseDto>>("GetScannedProducts", parameters))?.Result;

		_scanModel = new ScanRequestDto()
		{
			p_id = result!.p_id,
			p_barcode = result.p_barcode,
			p_title = result.p_title,
			p_unit = result.p_unit,
			p_companyid = result.p_companyid,
			p_imageurl = result.p_imageurl,
			p_salesprice = result.p_salesprice,
			p_costprice = result.p_costprice,
		};
			_showUpsertScanDialog = true;
		}
	else
	{
		_scanModel = new ScanRequestDto();
			_showUpsertScanDialog = true;
		}
}
 private async Task OnUpsertScanDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showUpsertScanDialog = false;
        }
        else
        {
            var jsonRequest = JsonSerializer.Serialize(_scanModel);

            var jsonContent = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            await BaseService.PostAsync<Derived<object>>("UpsertBarcodeProduct", jsonContent);

            _showUpsertScanDialog = false;
            
            await OnInitializedAsync();
        }
    }
}