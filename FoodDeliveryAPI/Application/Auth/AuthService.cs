using AutoMapper;
using FoodDeliveryAPI.Application.Auth.PasswordHash;
using FoodDeliveryAPI.Application.DTOs;
using FoodDeliveryAPI.Domains.Entities;
using FoodDeliveryAPI.Infrastructure.Repositories;
using FoodDeliveryAPI.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Components.Forms;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;

namespace FoodDeliveryAPI.Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordService _passwordService;

        public AuthService(ITokenService tokenService, IUnitOfWork unitOfWork, ILogger<AuthService> logger,
            IMapper mapper, IUsuarioRepository usuarioRepository, IConfiguration configuration, IPasswordService passwordService)
        {
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
            _passwordService = passwordService;
        }

        public async Task<UsuarioResponseDTO> RegisterAsync(UsuarioCreateDTO usuarioCreateDTO)
        {
            if(string.IsNullOrWhiteSpace(usuarioCreateDTO.Email) || string.IsNullOrWhiteSpace(usuarioCreateDTO.Senha) || string.IsNullOrWhiteSpace(usuarioCreateDTO.TipoUsuario))
            {
                _logger.LogWarning("Dados de registro incompletos para email {Email}.", usuarioCreateDTO.Email);
                throw new InvalidOperationException("Email, senha e tipo de usuário são obrigatórios.");
            }

            var buscarUsuario = await _usuarioRepository.GetByEmailAsync(usuarioCreateDTO.Email);

            if (buscarUsuario != null)
            {
                _logger.LogWarning("Usuário com email {Email} já existe.", usuarioCreateDTO.Email);
                throw new InvalidOperationException("Usuário com este email já existe.");
            }
            
            if(usuarioCreateDTO.Senha != usuarioCreateDTO.SenhaConfirmacao) 
            {
                _logger.LogWarning("Senha e confirmação de senha não coincidem para email {Email}.", usuarioCreateDTO.Email);
                throw new InvalidOperationException("Senha e confirmação de senha não coincidem.");
            }

            var novoUsuario = _mapper.Map<Usuario>(usuarioCreateDTO);

            // Hash de senha
            var senhaHash = _passwordService.HashPassword(usuarioCreateDTO.Senha);
            novoUsuario.Senha = senhaHash;

            await _usuarioRepository.AddAsync(novoUsuario);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Novo usuário registrado com email {Email}.", usuarioCreateDTO.Email);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, novoUsuario.Id.ToString()),
                new Claim(ClaimTypes.Email, novoUsuario.Email),
            };

            var tipo = novoUsuario.Tipo.ToLower();

            if (tipo == "cliente" || tipo == "entregador")
            {
                claims.Add(new Claim(ClaimTypes.Role, tipo));
            }
            else
            {
                _logger.LogWarning("Tipo de usuário desconhecido {Tipo} para email {Email}.", novoUsuario.Tipo, usuarioCreateDTO.Email);
                throw new InvalidOperationException("Tipo de usuário desconhecido.");
            }

            var token = _tokenService.GenerateToken(claims, _configuration);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new UsuarioResponseDTO
            {
                Email = novoUsuario.Email,
                TipoUsuario = novoUsuario.Tipo,
                Token = tokenString,
                RefreshToken = refreshToken
            };
        }

        public async Task<UsuarioResponseDTO> LoginAsync(UsuarioLoginDTO usuarioLoginDTO)
        {
           var buscaUsuario = await _usuarioRepository.GetByEmailAsync(usuarioLoginDTO.Email);

            if (buscaUsuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado.");
            }

            var verificaSenha = _passwordService.Verification(usuarioLoginDTO.Senha, buscaUsuario.Senha);

            if (!verificaSenha)
            {
                throw new InvalidOperationException("Email ou senha inválidos.");
            }

            var tipo = buscaUsuario.Tipo.ToLower();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, buscaUsuario.Id.ToString()),
                new Claim(ClaimTypes.Email, buscaUsuario.Email),
                new Claim(ClaimTypes.Role, tipo)
            };

            var token = _tokenService.GenerateToken(claims, _configuration);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _logger.LogInformation("Usuário com email {Email} logado com sucesso.", usuarioLoginDTO.Email);

            return new UsuarioResponseDTO
            {
                Email = buscaUsuario.Email,
                TipoUsuario = buscaUsuario.Tipo,
                Token = tokenString,
                RefreshToken = refreshToken
            };
        }

       
    }
}
