import { resourcesWithoutMaps } from './constants'

export const getResourceTypeData = (resourceTypes, resourceTypeId) => {
    return resourceTypes.find(resourceType => resourceTypeId.toLowerCase() === resourceType.value.toLowerCase()) || {};
}

export const shouldHideMap = (resourceType) => {
    return resourcesWithoutMaps.find(resource => resource.toLowerCase() === resourceType.toLowerCase())
}
