import { resourcesWithoutMaps } from './constants'

export const shouldHideMap = (resourceType) => resourcesWithoutMaps.find(resource => resource === resourceType)
