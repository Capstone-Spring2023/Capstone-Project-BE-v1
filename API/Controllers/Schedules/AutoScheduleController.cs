using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using AutoScheduling.Reader;
using Swashbuckle.AspNetCore.Annotations;
using AutoScheduling;
using System.ComponentModel.DataAnnotations;
using AutoScheduling.DataLayer;
using Data.Models;

namespace API.Controllers.Schedules
{
    [Route("api/auto-schedule")]
    [ApiController]
    public class AutoScheduleController : ControllerBase
    {
        private readonly CFManagementContext _context;
        public AutoScheduleController(CFManagementContext context)
        {
            _context = context;
        }
        [HttpPost("import-file/semester/{semesterId}")]
        [SwaggerOperation(Summary = "Import csv file từ phòng đào tạo")]
        public async Task<IActionResult> ClassDaySlotReaderAPI([FromForm][Required] IFormFile[] files, [Required][FromRoute] int semesterId)
        {
            Checker checker = new Checker();    
            var check =checker.checkIfThereAlreadyAvailableSubject(semesterId);
            if (check)
            {
                return BadRequest("Schedule have already imported");
            }
            ClassDaySlotReader classDaySlotReader = new ClassDaySlotReader();
            var csvFile = files[0];
            if (csvFile == null)
            {
                return BadRequest();
            }
            await classDaySlotReader.readClassDaySlotCsvToDb(csvFile,semesterId);
            var listUserIdOfLecturerAndLeader = _context.Users.Where(x => x.RoleId == 2 || x.RoleId == 3).ToList();
            foreach (var user in listUserIdOfLecturerAndLeader)
            {
                var notification = new Notification();
                notification.Type = "schedule";
                notification.UserId = user.UserId;
                notification.Message = "Class registration has been open, please register before";
                notification.Sender = null;
                notification.SubjectCode = null;
                notification.Status = "Unread";
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }
            return Ok("Create Success");
        }
        [HttpPost("out-of-flow")]
        [SwaggerOperation(Summary = "Import csv file register subject vào database (ngược với flow hiện tại)")]
        public async Task<IActionResult> outOfFlow(IFormFile[] file)
        {

            OutOfFlow outOfFlow = new OutOfFlow();
            
            await outOfFlow.createRegisterSubjectDatabaseFromFile(file[0]);
            return Ok("Kê");
        }
        [HttpPost("out-of-flow-1")]
        [SwaggerOperation(Summary = "Import csv file register subject vào database (ngược với flow hiện tại)")]
        public async Task<IActionResult> outOfFlow1(IFormFile[] file)
        {

            OutOfFlow outOfFlow = new OutOfFlow();

            await outOfFlow.createAbleSubjectDatabase(file[0]);
            return Ok("Kê");
        }
        [HttpGet("get-file")]
        [SwaggerOperation(Summary = "Lấy csv file đăng ký từ hệ thống")]
        public async Task<ObjectResult> registerSubjectReaderAPi([Required][FromQuery] int semesterId)
        {
            
            RegisterSubjectReader registerSubjectReader = new RegisterSubjectReader();

             registerSubjectReader.createRegisterSubjectFileFromDatabase(semesterId);
            
            //await classDaySlotReader.readClassDaySlotCsvToDb(csvFile);
            string filePath = @"register_subject_v1.csv";
            var stream = new MemoryStream(System.IO.File.ReadAllBytes(filePath).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "file", filePath.Split(@"\").Last());
            //Response.SendFileAsync((Microsoft.Extensions.FileProviders.IFileInfo)file);
            var link = await uploadFile(formFile);
            return new ObjectResult(link);
        }
        [HttpPost("main-flow-full")]
        [SwaggerOperation(Summary = "Tạo lịch nhưng với điều kiện TẤT CẢ các lớp đều có người dạy")]
        public async Task<IActionResult> mainFlowButFull(IFormFile[] file, [Required][FromQuery] int semesterId)
        {
            if (file.Length < 2) return BadRequest();
            AutoSchedulingMain a = new AutoSchedulingMain();
            Last_Constraint @delegate = new Last_Constraint(MainFlowFunctions.everyClassHaveTeacher);

            var check = a.MainFlow(file[0], file[1], @delegate,semesterId);

            if (!check) return BadRequest("Cannot create Schedule with this data");
            else
            {
                var filePath = Constant.SCHEDULE_FILE;
                var stream = new MemoryStream(System.IO.File.ReadAllBytes(filePath).ToArray());
                var formFile = new FormFile(stream, 0, stream.Length, "file", filePath.Split(@"\").Last());
                //Response.SendFileAsync((Microsoft.Extensions.FileProviders.IFileInfo)file);
                var link = await uploadFile(formFile);
                ScheduleReader reader = new ScheduleReader();
                await reader.fromScheduleFile_writeToDatabase(formFile, file[0], semesterId);
                Updater updater = new Updater();
                updater.DeleteRedundantRegisterSubject();
                return Ok(link);
            }
        }
        [HttpPut("import-schedule-file")]
        [SwaggerOperation(Summary = "Import lịch schedule.csv ở 1 trong 2 api trên vào")]
        public async Task<IActionResult> importSchedule([FromForm]IFormFile[] file,[Required] [FromQuery] int semesterId)
        {
            if (file.Length <= 1) return BadRequest();
            ScheduleReader reader = new ScheduleReader();
            try
            {
                await reader.fromScheduleFile_writeToDatabase(file[0],file[1],semesterId);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Import Success");
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
