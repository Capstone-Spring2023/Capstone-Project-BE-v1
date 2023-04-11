using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using AutoScheduling.Reader;
using Swashbuckle.AspNetCore.Annotations;
using AutoScheduling;

namespace API.Controllers.Schedules
{
    [Route("api/auto-schedule")]
    [ApiController]
    public class AutoScheduleController : ControllerBase
    {
        [HttpPost("import-file")]
        [SwaggerOperation(Summary = "Import csv file từ phòng đào tạo")]
        public async Task<IActionResult> ClassDaySlotReaderAPI([FromForm] IFormFile[] files)
        {
            ClassDaySlotReader classDaySlotReader = new ClassDaySlotReader();
            var csvFile = files[0];
            if (csvFile == null)
            {
                return BadRequest();
            }
            await classDaySlotReader.readClassDaySlotCsvToDb(csvFile);
            return Ok("Create Success");
        }
        [HttpPost("get-file")]
        [SwaggerOperation(Summary = "Lấy csv file đăng ký từ hệ thống")]
        public async Task<ObjectResult> registerSubjectReaderAPi()
        {
            
            RegisterSubjectReader registerSubjectReader = new RegisterSubjectReader();
             registerSubjectReader.createRegisterSubjectFileFromDatabase();
            //await classDaySlotReader.readClassDaySlotCsvToDb(csvFile);
            string filePath = @"register_subject_v1.csv";
            var stream = new MemoryStream(System.IO.File.ReadAllBytes(filePath).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "file", filePath.Split(@"\").Last());
            //Response.SendFileAsync((Microsoft.Extensions.FileProviders.IFileInfo)file);
            var link = await uploadFile(formFile);
            return new ObjectResult(link);
        }
        [HttpPost("main-flow")]
        [SwaggerOperation(Summary = "Tạo lịch")]
        public async Task<IActionResult> mainFlow(IFormFile[] file)
        {
            if (file.Length < 2) return BadRequest();
            AutoSchedulingMain a = new AutoSchedulingMain();
            Last_Constraint @delegate = new Last_Constraint(MainFlowFunctions.noDuplicateClass);
            var check = a.MainFlow(file[0], file[1], @delegate);
            if (!check) return BadRequest("Cannot create Schedule with this data");
            else
            {
                var filePath = Constant.SCHEDULE_FILE;
                var stream = new MemoryStream(System.IO.File.ReadAllBytes(filePath).ToArray());
                var formFile = new FormFile(stream, 0, stream.Length, "file", filePath.Split(@"\").Last());
                //Response.SendFileAsync((Microsoft.Extensions.FileProviders.IFileInfo)file);
                var link = await uploadFile(formFile);
                return Ok(link);
            }
        }
        [HttpPost("main-flow-full")]
        [SwaggerOperation(Summary = "Tạo lịch nhưng với điều kiện TẤT CẢ các lớp đều có người dạy")]
        public async Task<IActionResult> mainFlowButFull(IFormFile[] file)
        {
            if (file.Length < 2) return BadRequest();
            AutoSchedulingMain a = new AutoSchedulingMain();
            Last_Constraint @delegate = new Last_Constraint(MainFlowFunctions.everyClassHaveTeacher);
            var check = a.MainFlow(file[0], file[1], @delegate);
            if (!check) return BadRequest("Cannot create Schedule with this data");
            else
            {
                var filePath = Constant.SCHEDULE_FILE;
                var stream = new MemoryStream(System.IO.File.ReadAllBytes(filePath).ToArray());
                var formFile = new FormFile(stream, 0, stream.Length, "file", filePath.Split(@"\").Last());
                //Response.SendFileAsync((Microsoft.Extensions.FileProviders.IFileInfo)file);
                var link = await uploadFile(formFile);
                return Ok(link);
            }
        }
        private BlobContainerClient GetBlobContainerClient()
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=eoolykos;AccountKey=Wfvf3jeVcajjl89QhvXGczlD4Y4/ZxUqQUkQW0UpMYrIiDnsFhPtffrn5qTHOAQoxSdKcLWgC57i+AStvWx23g==;EndpointSuffix=core.windows.net";
            string containerName = "mustang";
            return new BlobContainerClient(connectionString, containerName);
        }
        private async Task<String> uploadFile(IFormFile file)
        {
            var container = GetBlobContainerClient();
            try
            {
                var blobClient = container.GetBlobClient(file.FileName);
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    ms.Position = 0;
                    var blobHttpHeader = new BlobHttpHeaders { ContentType = "text/csv" };
                    await blobClient.UploadAsync(ms, new BlobUploadOptions { HttpHeaders = blobHttpHeader });
                    ;
                }
                return "https://eoolykos.blob.core.windows.net/mustang/" + file.FileName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
