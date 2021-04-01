import React, { useEffect, useState } from 'react'
import PropTypes from 'prop-types'
import { Container } from 'app/components/forms/index'
import { Resource } from './resource'
import useRequireLogin from 'app/hooks/useRequireLogin';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faPlus, faMinus } from '@fortawesome/free-solid-svg-icons'

import styles from './my-rebellion.module.scss';

const MY_REBELLION = "My Rebellion Subscriptions"

function getSubscriptionTree(subscriptions, resourceTypes, level = 'root', closed, onToggleList) {
	return (
		<ul className={`${styles.tree} ${styles[level]}`}>
			{subscriptions.map(subscription => {
				const isClosed = closed.includes(subscription.reference.guid);
				const resourceType = resourceTypes.find(thisType => thisType.value === subscription.reference.metadata.type)
				const backgroundColor = resourceType && '#' + resourceType.displayColor
				const hasChildren = subscription.children.length > 0

				return (
					<React.Fragment key={subscription.reference.guid}>
						<li>
							<button
								className={styles.button}
								style={{backgroundColor: hasChildren ? backgroundColor : undefined}}
								onClick={() => onToggleList(subscription.reference.guid, !isClosed)}
								disabled={!hasChildren}
								aria-hidden={true}
								title={isClosed ? 'Expand' : 'Collapse'}>
								<FontAwesomeIcon icon={isClosed ? faPlus : faMinus} />
							</button>
							<Resource
								{...subscription.reference}
								resourceTypes={resourceTypes}
								administrator={subscription.administrator}
								member={subscription.member}
							/>
						</li>
						{subscription.children.length > 0 && (
							<li className={isClosed ? styles.hidden : ''}>
								{getSubscriptionTree(subscription.children, resourceTypes, 'child', closed, onToggleList)}
							</li>
						)}
					</React.Fragment>
				);
			})}
		</ul>
	);
}

export const MyRebellion = ({ error, getMyRebellion, isFetching, myRebellion = [], resourceTypes }) => {
	useEffect(() => { getMyRebellion() }, [getMyRebellion]);

	const [closed, setClosed] = useState([]);

	// Ensure user is logged in
	const loggedIn = useRequireLogin();
	if(!loggedIn) return null;

	const onToggleList = (guid, closing) => {
		if(closing) {
			setClosed([...closed, guid]);
		} else {
			setClosed(closed.filter(listGuid => listGuid !== guid));
		}
	}

	let content;
	if(isFetching) {
		content = <p></p>;
	}
	else if(error) {
		content = <>There was an error loading the data.</>;
	}
	else {
		content = <>
			{myRebellion.length ? (
				<>{getSubscriptionTree(myRebellion, resourceTypes, 'root', closed, onToggleList)}</>
			) : (
				<p>You do not currently have any subscriptions.</p>
			)}
		</>;
	}

	return (
		<Container
			title={MY_REBELLION}
			loading={isFetching}
			content={content}
		/>
	)
}

MyRebellion.propTypes = {
	user: PropTypes.object
}

export default MyRebellion;
