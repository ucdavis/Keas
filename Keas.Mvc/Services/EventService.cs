﻿using Keas.Core.Domain;
using System.Threading.Tasks;

namespace Keas.Mvc.Services
{
    public interface IEventService
    {
        Task TrackCreateKey(Key key, User user);
        Task TrackAssignKey(Key key, User user);
        Task TrackUnAssignKey(Key key, User user);
        Task TrackCreateEquipment(Equipment equipment, User user);
        Task TrackAssignEquipment(Equipment equipment, User user);
        Task TrackUnAssignEquipment(Equipment equipment, User user);

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

        public async Task TrackCreateKey(Key key, User user)
        {
            var history = await _historyService.KeyCreated(key, user);
            await _notificationService.KeyCreatedUpdatedInactive(key, history);
        }

        public async Task TrackAssignKey(Key key, User user)
        {
            var history = await _historyService.KeyAssigned(key, user);
            await _notificationService.KeyAssigned(key, history);

        }

        public async Task TrackUnAssignKey(Key key, User user)
        {
            var history = await _historyService.KeyUnassigned(key, user);
            await _notificationService.KeyUnAssigned(key, history);
        }

        public async Task TrackCreateEquipment(Equipment equipment, User user)
        {
            var history = await _historyService.EquipmentCreated(equipment, user);
            await _notificationService.EquipmentCreatedUpdatedInactive(equipment, history);
        }

        public async Task TrackAssignEquipment(Equipment equipment, User user)
        {
            var history = await _historyService.EquipmentAssigned(equipment, user);
            await _notificationService.EquipmentAssigned(equipment, history);
        }

        public async Task TrackUnAssignEquipment(Equipment equipment, User user)
        {
            var history = await _historyService.EquipmentUnassigned(equipment, user);
            await _notificationService.EquipmentUnAssigned(equipment, history);
        }
    }
}

