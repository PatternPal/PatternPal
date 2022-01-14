using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IDesign.LoggingAPI;
using IDesign.LoggingAPI.DAL;

namespace IDesign.LoggingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public LoggingController(LoggingContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }
        #region Session
        // POST: api/Logging/Sessions
        [HttpPost("Sessions")]
        public ActionResult<Session> PostSession(Session session)
        {
            if (session.EndSession != default && session.EndSession < session.StartSession)
                return BadRequest();
            _unitOfWork.SessionRepository.Add(session);
            _unitOfWork.Save();
            return _unitOfWork.SessionRepository.GetById(session.ID); 
        }

        // PUT: api/Logging/Sessions/EndSession/5
        [HttpPut("Sessions/EndSession/{id}")]
        public IActionResult PutEndSession(Guid id, DateTime end)
        {
            try
            {
                _unitOfWork.SessionRepository.ChangeEndSession(id, end);
            }
            catch (Exception)
            {
                return BadRequest(); //either a session with the id wasn't found, or the endTime is before startTime
            }
            try
            {
                _unitOfWork.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_unitOfWork.SessionRepository.GetById(id) == null)
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }
        #endregion
        #region Action
        // POST: api/Logging/Actions
        [HttpPost("Actions")]
        public ActionResult<Action> PostAction(Action action)
        {
            Session actionSession = _unitOfWork.SessionRepository.GetById(action.SessionID);
            if (actionSession.EndSession != default && action.Time > actionSession.EndSession || action.Time < actionSession.StartSession)
                return BadRequest();
            _unitOfWork.ActionRepository.Add(action);
            _unitOfWork.Save();
            //return CreatedAtAction("GetAction", new { id = action.Id }, action);
            return _unitOfWork.ActionRepository.GetById(action.ID);
        }

        #endregion
        #region ActionType
        // POST: api/Logging/ActionTypes
        [HttpPost("ActionTypes")]
        public ActionResult<ActionType> PostActionType(ActionType actionType)
        {
            _unitOfWork.ActionTypeRepository.Add(actionType);
            _unitOfWork.Save();
            return _unitOfWork.ActionTypeRepository.GetById(actionType.ID);
        }
        #endregion
        #region Mode
        // POST: api/Logging/Modes
        [HttpPost("Modes")]
        public ActionResult<Mode> PostMode(Mode mode)
        {
            _unitOfWork.ModeRepository.Add(mode);
            _unitOfWork.Save();
            return _unitOfWork.ModeRepository.GetById(mode.ID);
        }

        
        #endregion
        #region ExtensionErrors
        // POST: api/Logging/ExtensionErrors
        [HttpPost("ExtensionErrors")]
        public ActionResult<ExtensionError> PostExtensionError(ExtensionError extensionError)
        {
            _unitOfWork.ExtensionErrorRepository.Add(extensionError);
            _unitOfWork.Save();
            return _unitOfWork.ExtensionErrorRepository.GetById(extensionError.ID);
        }

        // PUT: api/Logging/ExtensionErrors/5
        [HttpPut("ExtensionErrors/{id}")]
        public IActionResult PutExtensionError(int id, ExtensionError extensionError)
        {
            if (id != extensionError.ID)
            {
                return BadRequest();
            }

            _unitOfWork.ExtensionErrorRepository.Update(extensionError);
            _unitOfWork.Save();
            return NoContent();
        }
        #endregion
    }
}
