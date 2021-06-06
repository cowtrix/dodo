import { resourcesWithoutMaps } from './constants'

export const getResourceTypeData = (resourceTypes, resourceTypeId) => {
    return resourceTypes.find(resourceType => resourceTypeId.toLowerCase() === resourceType.value.toLowerCase()) || {};
}

export const shouldHideMap = (resourceType) => {
    return resourcesWithoutMaps.find(resource => resource.toLowerCase() === resourceType.toLowerCase())
}

export const titleCase = (text) => {
    return text.toLowerCase().replace(/\b(\w)/g, s => s.toUpperCase());
}
