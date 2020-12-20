import React, { useEffect, useState } from 'react'
import PropTypes from 'prop-types'
import { Container } from 'app/components/forms/index'
import { Resource } from './resource'

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faPlus, faMinus } from '@fortawesome/free-solid-svg-icons'

import styles from './my-rebellion.module.scss';

const MY_REBELLION = "My rebellion"

function getSubscriptionTree(subscriptions, resourceTypes, level = 'root', closed, onToggleList) {
	return (
		<ul className={`${styles.tree} ${styles[level]}`}>
			{subscriptions.map(subscription => {
				const isClosed = closed.includes(subscription.reference.guid);

				return (
					<>
						<li>
							<button
								className={styles.button}
								onClick={() => onToggleList(subscription.reference.guid, !isClosed)}
								disabled={subscription.children.length === 0}
								aria-hidden={true}>
								<FontAwesomeIcon icon={isClosed ? faPlus : faMinus} />
							</button>
							<Resource
								{...subscription.reference}
								resourceTypes={resourceTypes}
								administrator={subscription.administrator}
							/>
						</li>
						{subscription.children.length > 0 && (
							<li className={isClosed ? styles.hidden : ''}>
								{getSubscriptionTree(subscription.children, resourceTypes, 'child', closed, onToggleList)}
							</li>
						)}
					</>
				);
			})}
		</ul>
	);
}

export const MyRebellion = ({ error, getMyRebellion, isFetching, myRebellion = [], resourceTypes }) => {
	useEffect(() => { getMyRebellion() }, []);

	const [closed, setClosed] = useState([]);

	const onToggleList = (guid, closing) => {
		if(closing) {
			setClosed([...closed, guid]);
		} else {
			setClosed(closed.filter(listGuid => listGuid !== guid));
		}
	}

	let content;
	if(isFetching) {
		content = <p>Loading, please wait...</p>;
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
		<Container title={MY_REBELLION} content={content} />
	)
}

MyRebellion.propTypes = {
	user: PropTypes.object
}

export default MyRebellion;
