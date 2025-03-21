﻿using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/v{version:apiversion}/UserAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        protected APIResponse _response;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            this._response = new APIResponse();
        }

        [HttpPost("Login")]
        public async Task<ActionResult> LogIn([FromBody] LogInRequestDTO logInRequestDTO)
        {
            LogInResponseDTO Response = await _userRepository.LogIn(logInRequestDTO);

            if (Response.LocalUser == null || String.IsNullOrEmpty(Response.AccessToken))
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User name or password is incorrect");
                _response.StatusCode = HttpStatusCode.BadRequest;

                return BadRequest(_response);
            }

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = Response;

            return Ok(_response);
        }


        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterationRequestDTO registerationRequestDTO)
        {
            bool IsUnique = _userRepository.IsUnique(registerationRequestDTO.UserName);

            if (IsUnique == false)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User name already exists");
                _response.StatusCode = HttpStatusCode.BadRequest;

                return BadRequest(_response);
            }


            UserDTO Response = await _userRepository.Register(registerationRequestDTO);

            if (Response == null)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Erorr while registering");
                _response.StatusCode = HttpStatusCode.BadRequest;

                return BadRequest(_response);
            }

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = Response;

            return Ok(_response);
        }


        [HttpPost("Refresh")]
        public async Task<ActionResult> GetNewTokenFromRefreshToken([FromBody] LogInResponseDTO logInResponseDTO)
        {

            if (ModelState.IsValid)
            {
                var TokenResponse = await _userRepository.RefreshAccessToken(logInResponseDTO);

                if (TokenResponse == null || string.IsNullOrEmpty(TokenResponse.RefreshToken))
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Invalid Token");
                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);
                }


                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = TokenResponse;

                return Ok(_response);
            }

            else
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Invalid Input");
                _response.StatusCode = HttpStatusCode.BadRequest;

                return BadRequest(_response);
            }

        }

        [HttpPost("Revoke")]
        public async Task<ActionResult> ReVokeRefreshToken([FromBody] LogInResponseDTO logInResponseDTO)
        {

            if (ModelState.IsValid)
            {
                await _userRepository.RevokeRefreshToken(logInResponseDTO);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }

            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Invaild Input");
            _response.StatusCode = HttpStatusCode.BadRequest;

            return BadRequest(_response);

        }

    }
}
