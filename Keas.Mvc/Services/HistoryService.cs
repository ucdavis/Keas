﻿using System.Linq;
using Keas.Core.Data;
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
        Task<History> KeyAssignedSerial(KeySerial keySerial);
        Task<History> AccessAssigned(AccessAssignment accessAssignment);
        Task<History> EquipmentAssigned(Equipment equipment);
        Task<History> KeySerialUnassigned(KeySerial keySerial);
        Task<History> AccessUnassigned(AccessAssignment accessAssignment);
        Task<History> EquipmentUnassigned(Equipment equipment);
        Task<History> KeySerialAccepted(KeySerial keySerial);
        Task<History> AccessAccepted(Access access);
        Task<History> EquipmentAccepted(Equipment equipment);
        Task<History> WorkstationCreated(Workstation workstation);
        Task<History> WorkstationUpdated(Workstation workstation);
        Task<History> WorkstationInactivated(Workstation workstation);
        Task<History> WorkstationAssigned(Workstation workstation);
        Task<History> WorkstationUnassigned(Workstation workstation);
        Task<History> WorkstationAccepted(Workstation workstation);

        Task<History> KeyDeleted(Key key);
        Task<History> AccessDeleted(Access access);
        Task<History> EquipmentDeleted(Equipment equipment);
        Task<History> WorkstationDeleted(Workstation workstation);

        Task<History> KeySerialAssignmentUpdated(KeySerial keySerial);
        Task<History> AccessAssignmentUpdated(AccessAssignment accessAssignment);
        Task<History> EquipmentAssignmentUpdated(Equipment equipment);
        Task<History> WorkstationAssignmentUpdated(Workstation workstation);


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
                Description = key.GetDescription(nameof(key), key.Title, user, "Created"),
                ActorId = user.Id,
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
                Description = access.GetDescription(nameof(access), access.Title, user, "Created"),
                ActorId = user.Id,
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
                Description = "Equipment (" + equipment.Name + ") Created by " + user.Name,
                ActorId = user.Id,
                AssetType = "Equipment",
                ActionType = "Created",
                Equipment = equipment,
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
                Description = key.Name != null ? "Key (" + key.Name + ") Updated by " + user.Name : "Key (" + key.Code + ") Updated by " + user.Name,
                ActorId = user.Id,
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
                Description = "Access (" + access.Name + ") Updated by " + user.Name,
                ActorId = user.Id,
                AssetType = "Access",
                ActionType = "Updated",
                Access = access,
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
                Description = "Equipment (" + equipment.Name + ") Updated by " + user.Name,
                ActorId = user.Id,
                AssetType = "Equipment",
                ActionType = "Updated",
                Equipment = equipment,
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
                Description = key.Name != null ? "Key (" + key.Name + ") Inactivated by " + user.Name : "Key (" + key.Code + ") Inactivated by " + user.Name,
                ActorId = user.Id,
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
                Description = "Access (" + access.Name + ") Inactivated by " + user.Name,
                ActorId = user.Id,
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
                Description = "Equipment (" + equipment.Name + ") Inactivated by " + user.Name,
                ActorId = user.Id,
                AssetType = "Equipment",
                ActionType = "Inactivated",
                Equipment = equipment
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> KeyAssignedSerial(KeySerial keySerial)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = keySerial.Key.Name != null ? "Key (" + keySerial.Key.Name + ") Assigned to " + keySerial.Assignment.Person.User.Name + " by " + user.Name : "Key (" + keySerial.Key.Code + ") Assigned to " + keySerial.Assignment.Person.User.Name + " by " + user.Name,
                ActorId = user.Id,
                AssetType = "Key",
                ActionType = "Assigned",
                KeySerial = keySerial,
                TargetId = keySerial.Assignment.PersonId
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
                Description = "Access (" + accessAssignment.Access.Name + ") Assigned to " + accessAssignment.Person.User.Name + " by " + user.Name,
                ActorId = user.Id,
                AssetType = "Access",
                ActionType = "Assigned",
                AccessId = accessAssignment.AccessId,
                TargetId = accessAssignment.PersonId
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
                Description = "Equipment (" + equipment.Name + ") Assigned to " + equipment.Assignment.Person.User.Name + " by " + user.Name,
                ActorId = user.Id,
                AssetType = "Equipment",
                ActionType = "Assigned",
                Equipment = equipment,
                TargetId = equipment.Assignment.PersonId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> KeySerialUnassigned(KeySerial keySerial)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = keySerial.Key.Name != null ? "Key (" + keySerial.Key.Name + ") Unassigned from " + keySerial.Assignment.Person.User.Name + " by " + user.Name : "Key (" + keySerial.Key.Code + ") Unassigned  by " + user.Name,
                ActorId = user.Id,
                AssetType = "Key",
                ActionType = "Unassigned",
                Key = keySerial.Key,
                KeySerial = keySerial,
                TargetId = keySerial.Assignment.PersonId
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
                Description = "Access (" + accessAssignment.Access.Name + ") Unassigned from " + accessAssignment.Person.User.Name + " by " + user.Name,
                ActorId = user.Id,
                AssetType = "Access",
                ActionType = "Unassigned",
                AccessId = accessAssignment.AccessId,
                TargetId = accessAssignment.PersonId
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
                Description = "Equipment (" + equipment.Name + ") Unassigned from " + equipment.Assignment.Person.User.Name + " by " + user.Name,
                ActorId = user.Id,
                AssetType = "Equipment",
                ActionType = "Unassigned",
                Equipment = equipment,
                TargetId = equipment.Assignment.PersonId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> KeySerialAccepted(KeySerial keySerial)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = keySerial.Key.Name != null ? "Key (" + keySerial.Key.Name + ") Accepted by " + user.Name : "Key (" + keySerial.Key.Code + ") Accepted by " + user.Name,
                ActorId = user.Id,
                AssetType = "Key",
                ActionType = "Accepted",
                Key = keySerial.Key,
                KeySerial = keySerial,
                TargetId = keySerial.Assignment.PersonId
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
                Description = "Access (" + access.Name + ") Accepted by " + user.Name,
                ActorId = user.Id,
                AssetType = "Access",
                ActionType = "Accepted",
                Access = access,
                //TargetId = null //TODO: Get and set? Currently this method isn't being called.
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
                Description = "Equipment (" + equipment.Name + ") Accepted by " + user.Name,
                ActorId = user.Id,
                AssetType = "Equipment",
                ActionType = "Accepted",
                Equipment = equipment,
                TargetId = equipment.Assignment.PersonId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> WorkstationCreated(Workstation workstation)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Workstation (" + workstation.Name + ") Created by " + user.Name,
                ActorId = user.Id,
                AssetType = "Workstation",
                ActionType = "Created",
                Workstation = workstation,
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> WorkstationUpdated(Workstation workstation)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Workstation (" + workstation.Name + ") Updated by " + user.Name,
                ActorId = user.Id,
                AssetType = "Workstation",
                ActionType = "Updated",
                Workstation = workstation,
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> WorkstationInactivated(Workstation workstation)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Workstation (" + workstation.Name + ") Inactivated by " + user.Name,
                ActorId = user.Id,
                AssetType = "Workstation",
                ActionType = "Inactivated",
                Workstation = workstation
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> WorkstationAssigned(Workstation workstation)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Workstation (" + workstation.Name + ")  Assigned to " + workstation.Assignment.Person.User.Name + " by " + user.Name,
                ActorId = user.Id,
                AssetType = "Workstation",
                ActionType = "Assigned",
                Workstation = workstation,
                TargetId = workstation.Assignment.PersonId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> WorkstationUnassigned(Workstation workstation)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Workstation (" + workstation.Name + ")  Unassigned from " + workstation.Assignment.Person.User.Name + "  by " + user.Name,
                ActorId = user.Id,
                AssetType = "Workstation",
                ActionType = "Unassigned",
                Workstation = workstation,
                TargetId = workstation.Assignment.PersonId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> WorkstationAccepted(Workstation workstation)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Workstation (" + workstation.Name + ")  Accepted by " + user.Name,
                ActorId = user.Id,
                AssetType = "Workstation",
                ActionType = "Accepted",
                Workstation = workstation,
                TargetId = workstation.Assignment.PersonId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> KeyDeleted(Key key)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = key.Name != null ? "Key (" + key.Name + ") Deleted by " + user.Name : "Key (" + key.Code + ") Deleted by " + user.Name,
                ActorId = user.Id,
                AssetType = "Key",
                ActionType = "Deleted",
                Key = key
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }
        public async Task<History> AccessDeleted(Access access)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Access (" + access.Name + ") Deleted by " + user.Name,
                ActorId = user.Id,
                AssetType = "Access",
                ActionType = "Deleted",
                Access = access
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }
        public async Task<History> EquipmentDeleted(Equipment equipment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Equipment (" + equipment.Name + ") Deleted by " + user.Name,
                ActorId = user.Id,
                AssetType = "Equipment",
                ActionType = "Deleted",
                Equipment = equipment,
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }
        public async Task<History> WorkstationDeleted(Workstation workstation)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Workstation (" + workstation.Name + ") Deleted by " + user.Name,
                ActorId = user.Id,
                AssetType = "Workstation",
                ActionType = "Deleted",
                Workstation = workstation,
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }

        public async Task<History> KeySerialAssignmentUpdated(KeySerial keySerial)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = keySerial.Key.Name != null ? "Key (" + keySerial.Key.Name + ") Assignment to " + keySerial.Assignment.Person.User.Name + " Updated by " + user.Name : "Key (" + keySerial.Key.Code + ") Assignment to " + keySerial.Assignment.Person.User.Name + " Updated by " + user.Name,
                ActorId = user.Id,
                AssetType = "Key",
                ActionType = "AssignmentUpdated",
                KeySerial = keySerial,
                TargetId = keySerial.Assignment.PersonId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }
        public async Task<History> AccessAssignmentUpdated(AccessAssignment accessAssignment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Access (" + accessAssignment.Access.Name + ") Assignment to " + accessAssignment.Person.User.Name + " Updated by " + user.Name,
                ActorId = user.Id,
                AssetType = "Access",
                ActionType = "AssignmentUpdated",
                AccessId = accessAssignment.AccessId,
                TargetId = accessAssignment.PersonId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }
        public async Task<History> EquipmentAssignmentUpdated(Equipment equipment)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Equipment (" + equipment.Name + ") Assignment to " + equipment.Assignment.Person.User.Name + " Updated by " + user.Name,
                ActorId = user.Id,
                AssetType = "Equipment",
                ActionType = "AssignmentUpdated",
                Equipment = equipment,
                TargetId = equipment.Assignment.PersonId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }
        public async Task<History> WorkstationAssignmentUpdated(Workstation workstation)
        {
            var user = await _securityService.GetUser();
            var historyEntry = new History
            {
                Description = "Workstation (" + workstation.Name + ")  Assignment to " + workstation.Assignment.Person.User.Name + " Updated by " + user.Name,
                ActorId = user.Id,
                AssetType = "Workstation",
                ActionType = "AssignmentUpdated",
                Workstation = workstation,
                TargetId = workstation.Assignment.PersonId
            };
            _context.Histories.Add(historyEntry);
            await _context.SaveChangesAsync();
            return historyEntry;
        }
    }
}

