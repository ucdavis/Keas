﻿using Keas.Core.Data;
using Keas.Core.Domain;
using System.Threading.Tasks;

namespace Keas.Mvc.Services
{
    public interface IHistoryService
    {
        Task<History> KeyCreated(Key key);
        Task<History> AccessCreated(Access access);
        Task<History> EquipmentCreated(Equipment equipment);
        Task<History> KeyUpdated(Key key);
        Task<History> AccessUpdated(Access access);
        Task<History> EquipmentUpdated(Equipment equipment);
        Task<History> KeyInactivated(Key key);
        Task<History> AccessInactivated(Access access);
        Task<History> EquipmentInactivated(Equipment equipment);
        Task<History> KeyAssigned(Key key);
        Task<History> AccessAssigned(AccessAssignment accessAssignment);
        Task<History> EquipmentAssigned(Equipment equipment);
        Task<History> KeyUnassigned(Key key);
        Task<History> AccessUnassigned(AccessAssignment accessAssignment);
        Task<History> EquipmentUnassigned(Equipment equipment);
        Task<History> KeyAccepted(Key key);
        Task<History> AccessAccepted(Access access);
        Task<History> EquipmentAccepted(Equipment equipment);

    }

    public class HistoryService : IHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISecurityService _securityService;


        public HistoryService(ApplicationDbContext context, ISecurityService securityService)
        {
            _context = context;
            _securityService = securityService;
        }

        public async Task<History> KeyCreated(Key key)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Key Created by " + user.Name,
                Actor = user,
                AssetType = "Key",
                ActionType = "Created",
                Key = key
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> AccessCreated(Access access)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Access Created by " + user.Name,
                Actor = user,
                AssetType = "Access",
                ActionType = "Created",
                Access = access
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> EquipmentCreated(Equipment equipment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Equipment Created by " + user.Name,
                Actor = user,
                AssetType = "Equipment",
                ActionType = "Created",
                Equipment = equipment
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> KeyUpdated(Key key)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Key Updated by " + user.Name,
                Actor = user,
                AssetType = "Key",
                ActionType = "Updated",
                Key = key
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> AccessUpdated(Access access)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Access Updated by " + user.Name,
                Actor = user,
                AssetType = "Access",
                ActionType = "Updated",
                Access = access
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> EquipmentUpdated(Equipment equipment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Equipment Updated by " + user.Name,
                Actor = user,
                AssetType = "Equipment",
                ActionType = "Updated",
                Equipment = equipment
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }
        public async Task<History> KeyInactivated(Key key)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Key Inactivated by " + user.Name,
                Actor = user,
                AssetType = "Key",
                ActionType = "Inactivated",
                Key = key
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> AccessInactivated(Access access)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Access Inactivated by " + user.Name,
                Actor = user,
                AssetType = "Access",
                ActionType = "Inactivated",
                Access = access
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> EquipmentInactivated(Equipment equipment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Equipment Inactivated by " + user.Name,
                Actor = user,
                AssetType = "Equipment",
                ActionType = "Inactivated",
                Equipment = equipment
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> KeyAssigned(Key key)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Key Assigned to " + key.Assignment.Person.User.Name + " by " + user.Name,
                Actor = user,
                AssetType = "Key",
                ActionType = "Assigned",
                Key = key
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> AccessAssigned(AccessAssignment accessAssignment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Access Assigned to " + accessAssignment.Person.User.Name + " by " + user.Name,
                Actor = user,
                AssetType = "Access",
                ActionType = "Assigned",
                AccessId = accessAssignment.AccessId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> EquipmentAssigned(Equipment equipment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Equipment Assigned to " + equipment.Assignment.Person.User.Name + " by " + user.Name,
                Actor = user,
                AssetType = "Equipment",
                ActionType = "Assigned",
                Equipment = equipment
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> KeyUnassigned(Key key)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Key Unassigned  by " + user.Name,
                Actor = user,
                AssetType = "Key",
                ActionType = "Unassigned",
                Key = key
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> AccessUnassigned(AccessAssignment accessAssignment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Access Unassigned to " + accessAssignment.Person.User.Name + " by " + user.Name,
                Actor = user,
                AssetType = "Access",
                ActionType = "Unassigned",
                AccessId = accessAssignment.AccessId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> EquipmentUnassigned(Equipment equipment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Equipment Unassigned by " + user.Name,
                Actor = user,
                AssetType = "Equipment",
                ActionType = "Unassigned",
                Equipment = equipment
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> KeyAccepted(Key key)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Key Accepted by " + user.Name,
                Actor = user,
                AssetType = "Key",
                ActionType = "Accepted",
                Key = key
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> AccessAccepted(Access access)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Access Accepted by " + user.Name,
                Actor = user,
                AssetType = "Access",
                ActionType = "Accepted",
                Access = access
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> EquipmentAccepted(Equipment equipment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Equipment Accepted by " + user.Name,
                Actor = user,
                AssetType = "Equipment",
                ActionType = "Accepted",
                Equipment = equipment
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

    }
}

