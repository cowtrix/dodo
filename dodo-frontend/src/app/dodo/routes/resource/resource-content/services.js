export const isSubscribedToResource = (memberOf, resourceID) => memberOf.find(resource => resourceID === resource.guid)
