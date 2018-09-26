using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ViciniServer.Hardware;
using ViciniServer.Sessions;

namespace ViciniServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HardwareController : Controller
    {
        private ISessionManager sessionManager;

        public HardwareController(ISessionManager sessionManager) {
            this.sessionManager = sessionManager;
        }

        [HttpGet("{sid}")]
        public JsonResult Get(Guid sid)
        {
            var info = sessionManager.FindHardware(sid);
            var result = info.Select(
                i => new { hid = i.Id, controller = i.ControllerType, state = i.Status});
            return Json(new { sid = sid, hardware = result.ToArray() });
        }

        [HttpPut("{sid}/{hid}/reserve")]
        public JsonResult Reserve(Guid sid, string hid) {
            var hardwareId = new HardwareId(hid);
            var (details, info) = sessionManager.Reserve(sid, hardwareId);
            var detailsResult = new {
                id = details.Id,
                controller = details.ControllerType,
                boardName = details.BoardName,
                partName = details.PartName,
                otherPartNames = details.OtherPartNames };
            var hardwareResult = info.Select(
                i => new { hid = i.Id, controller = i.ControllerType, state = i.Status});
                return Json(new {
                    sid = sid,
                    hid = hid,
                    details = detailsResult,
                    hardware = hardwareResult.ToArray() });
        }

        [HttpPut("{sid}/{hid}/unreserve")]
        public JsonResult Unreserve(Guid sid, string hid) {
            var hardwareId = new HardwareId(hid);
            var info = sessionManager.Unreserve(sid, hardwareId);
            var hardwareResult = info.Select(
                i => new { hid = i.Id, controller = i.ControllerType, state = i.Status});
                return Json(new {
                    sid = sid,
                    hid = hid,
                    hardware = hardwareResult.ToArray() });
        }

        [HttpPut("{sid}/{hid}/send_command")]
        public async Task<JsonResult> Command(Guid sid, string hid, [FromBody] CommandRequest request)
        {
            var hardwareId = new HardwareId(hid);
            var response = await sessionManager.CommandRequestAsync(
                sid, hardwareId, request.command.name, request.command.args, request.timeout);
            if (response.TimedOut) {
                return TimeoutResponse(sid, hardwareId, "reading response");
            } else {
                return Json(new {
                    sid = sid,
                    hid = hid,
                    command = new { send = response.CommandString, receive = response.ResponseString }
                });
            }
        }

        private JsonResult TimeoutResponse(Guid sessionId, HardwareId id, string action)
        {
            return Json(new { 
                sid = sessionId,
                hid = id.Value,
                status = "Timeout",
                message = $"Time out while {action}"});
        }
    }
}
