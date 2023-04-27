using Canopy.API.Common;
using Canopy.API.Common.Models.DTO;
using Canopy.API.Common.Repositories;
using Canopy.Core.Common.Models;
using Canopy.Core.Common.Models.DTO;
using Canopy.Core.Common.Services.Interfaces;
using Canopy.Core.Common.Services.RPCCalls.RPCCalls;
using Microsoft.Data.SqlClient;
using ServiceBus.Interfaces;
using System.Data;
using System.Text.Json;

namespace Canopy.Core.Common.Services
{
    public class DataTableBulkEditService : IDataTableBulkEditService
    {
        #region Private Fields

        private readonly IServiceBus _serviceBus;
        private readonly IRepository<StateList> _repo;

        #endregion

        #region Public Constructor

        public DataTableBulkEditService(IServiceBus serviceBus, IRepository<StateList> repo)
        {
            _serviceBus = serviceBus;
            _repo = repo;
        }

        #endregion

        #region Public Methods

        public async Task<List<DropDownDTO>?> GetBatchEditFieldOptions(int appPageID, int layoutID)
        {
            var rpcCall = new DataTableColumnsRequestRPCCall(new DataTableColumnsRequestDTO(appPageID, layoutID, onlyBulkEditColumns: true));
            var response = await _serviceBus.PublishRPCMessageAsync(rpcCall);

            if (!string.IsNullOrEmpty(response))
            {
                var data = JsonSerializer.Deserialize<List<DropDownDTO>>(response);
                return data;
            }

            return null;
        }

        public async Task<IServiceResponse?> BatchEditUpdate(DataTableBatchEditUpdateDTO updateDTO)
        {
            try
            {
                var sqlParams = new List<SqlParameter>
                {
                    new SqlParameter { ParameterName = "@dataTableColumnID", Direction = ParameterDirection.Input, Value = updateDTO.DataTableColumnId },
                    new SqlParameter { ParameterName = "@newValue", Direction = ParameterDirection.Input, Value = updateDTO.NewValue },
                    new SqlParameter { ParameterName = "@tableIDs", Direction = ParameterDirection.Input, Value = string.Join(',', updateDTO.TableIds) }
                };

                var data = await _repo.ExecuteStoredProc("[core].[DataTableBulkEditorSave]", sqlParams);
                return null;
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponseDTO(ex.Message);
            }
        }

        #endregion
    }
}
