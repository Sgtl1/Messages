﻿using AutoMapper;
using MessagesCRUD.Application.Messages.Commands.CreateMessage;
using MessagesCRUD.Application.Messages.Commands.DeleteMessage;
using MessagesCRUD.Application.Messages.Commands.UpdateMessage;
using MessagesCRUD.Application.Messages.Queries;
using MessagesCRUD.Application.Messages.Queries.GetMessageList;
using MessagesCRUD.Domain;
using MessagesCRUD.WebApi.Models.Command;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MessagesCRUD.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    //[ApiVersionNeutral]
    [Produces("application/json")]
    [Route("api/{version:apiVersion}/[controller]")]
    public class MessageController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public MessageController(IMapper mapper, UserManager<AppUser> userManager) => 
            (_mapper, _userManager) = (mapper, userManager);

        /// <summary>
        /// Gets the list of messages
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// GET: /message
        /// </remarks>
        /// <returns>Returns MessageListVm</returns>
        /// <responce code="200">Success</responce>
        /// <responce code="401">If the user is unauthorized</responce>
        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MessageListVm>> GetAll()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var query = new GetMessageListQuery
            {
                UserId = user.Id
            };

            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        /// <summary>
        /// Gets the message list of the user
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// GET: /message/b3a296a9-f0de-417e-89d6-a23c1b4803fa
        /// </remarks>
        /// <param name="id">Id of the user</param>
        /// <returns>Returns MessageListVm</returns>
        /// <responce code="200">Success</responce>
        /// <responce code="401">If the user is unauthorized</responce>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MessageListVm>> GetAllOfUser(string id)
        {
            var query = new GetMessageListQuery
            {
                UserId = id
            };

            var vm = await Mediator.Send(query);
            return Ok(vm);
        }

        /// <summary>
        /// Creates the message
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// POST /message
        /// {
        ///     Text: "message text"
        /// }
        /// </remarks>
        /// <param name="createMessageDto">CreateMessageDto object</param>
        /// <returns>Returns id (guid)</returns>
        /// <responce code="201">Success</responce>
        /// <responce code="401">If the user is unauthorized</responce>
        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateMessageDto createMessageDto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var command = _mapper.Map<CreateMessageCommand>(createMessageDto);
            command.UserId = user.Id;

            var messageId = await Mediator.Send(command);
            return Ok(messageId);
        }

        /// <summary>
        /// Updates the message
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// PUT /message
        /// {
        ///     text: "update message text"
        /// }
        /// </remarks>
        /// <param name="updateMessageDto">UpdateMessageDto object</param>
        /// <returns>Returns NoContent</returns>
        /// <responce code="204">Success</responce>
        /// <responce code="401">If the user is unauthorized</responce>
        [HttpPut]
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromBody] UpdateMessageDto updateMessageDto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var command = _mapper.Map<UpdateMessageCommand>(updateMessageDto);
            command.UserId = user.Id;

            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Deletes the message by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// DELETE /message/2B40C5C1-C3C6-40B5-A6CB-8924FCE29298
        /// </remarks>
        /// <param name="id">Id of the message (guid)</param>
        /// <returns>Returns NoContent</returns>
        /// <responce code="204">Success</responce>
        /// <responce code="401">If the user is unauthorized</responce>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "User, Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var command = new DeleteMessageCommand
            {
                Id = id,
                UserId = user.Id
            };

            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Deletes the message of some user by id
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// DELETE /message/2B40C5C1-C3C6-40B5-A6CB-8924FCE29298
        /// </remarks>
        /// <param name="userId">Id of the message (guid)</param>
        /// <param name="messageId">Id of the user</param>
        /// <returns>Returns NoContent</returns>
        /// <responce code="204">Success</responce>
        /// <responce code="401">If the user is unauthorized</responce>
        [HttpDelete("delete/{userId}/{messageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string userId, Guid messageId)
        {
            var command = new DeleteMessageCommand
            {
                Id = messageId,
                UserId = userId
            };

            await Mediator.Send(command);
            return NoContent();
        }
    }
}
