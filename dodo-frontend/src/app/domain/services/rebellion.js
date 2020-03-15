import { api } from "app/domain/services/api-service";
import { REBELLIONS } from "app/domain/urls";

export const fetchRebellion = rebellionId => api(`${REBELLIONS}${rebellionId}`);
