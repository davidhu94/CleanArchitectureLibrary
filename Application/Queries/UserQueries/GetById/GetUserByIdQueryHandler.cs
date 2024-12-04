using Application.Interfaces.RepositoryInterfaces;
using Application.Queries.BookQueries.GetById;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.UserQueries.GetById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User>
    {
        private readonly IUserRepository _repository;

        public GetUserByIdQueryHandler(IUserRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var wantedUser = await _repository.GetByIdAsync(request.UserId);

            if (wantedUser == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} was not found.");
            }

            return wantedUser;
        }
    }
}
