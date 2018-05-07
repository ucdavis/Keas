﻿using Keas.Core.Domain;
using System.Threading.Tasks;

namespace Keas.Mvc.Services
{
    public interface IEventService
    {
        Task TrackCreateKey(Key key);
        Task TrackAssignKey(Key key);
        Task TrackUnAssignKey(Key key);
        Task TrackCreateEquipment(Equipment equipment);
        Task TrackAssignEquipment(Equipment equipment);
        Task TrackUnAssignEquipment(Equipment equipment);
        Task TrackCreateAccess(Access access);
        Task TrackAssignAccess(AccessAssignment accessAssignment, string teamName);
        Task TrackUnAssignAccess(AccessAssignment accessAssignment, string teamName);

    }
    public class EventService : IEventService
    {
        private readonly IHistoryService _historyService;
        private readonly INotificationService _notificationService;

        public EventService(IHistoryService historyService, INotificationService notificationService)
        {
            _historyService = historyService;
            _notificationService = notificationService;
        }

        public async Task TrackCreateKey(Key key)
        {
            var history = await _historyService.KeyCreated(key);
            await _notificationService.KeyCreatedUpdatedInactive(key, history);
        }

        public async Task TrackAssignKey(Key key)
        {
            var history = await _historyService.KeyAssigned(key);
            await _notificationService.KeyAssigned(key, history);

        }

        public async Task TrackUnAssignKey(Key key)
        {
            var history = await _historyService.KeyUnassigned(key);
            await _notificationService.KeyUnAssigned(key, history);
        }

        public async Task TrackCreateEquipment(Equipment equipment)
        {
            var history = await _historyService.EquipmentCreated(equipment);
            await _notificationService.EquipmentCreatedUpdatedInactive(equipment, history);
        }

        public async Task TrackAssignEquipment(Equipment equipment)
        {
            var history = await _historyService.EquipmentAssigned(equipment);
            await _notificationService.EquipmentAssigned(equipment, history);
        }

        public async Task TrackUnAssignEquipment(Equipment equipment)
        {
            var history = await _historyService.EquipmentUnassigned(equipment);
            await _notificationService.EquipmentUnAssigned(equipment, history);
        }

        public async Task TrackCreateAccess(Access access)
        {
            var history = await _historyService.AccessCreated(access);
            await _notificationService.AccessCreatedUpdatedInactive(access, history);
        }

        public async Task TrackAssignAccess(AccessAssignment accessAssignment, string teamName)
        {
            var history = await _historyService.AccessAssigned(accessAssignment);
            await _notificationService.AccessAssigned(accessAssignment, history, teamName);

        }

        public async Task TrackUnAssignAccess(AccessAssignment accessAssignment, string teamName)
        {
            var history = await _historyService.AccessUnassigned(accessAssignment);
            await _notificationService.AccessUnAssigned(accessAssignment, history, teamName);
        }

    }
}

