import React from 'react';
import { Icon } from 'app/components/icon';
import { Panel } from 'app/components/resources';
import { useTranslation } from "react-i18next";

import styles from './facilities.module.scss';

function camelToSentenceCase(text) {
	const sentence = text.replace( /([A-Z])/g, " $1" );
	return sentence.charAt(0).toUpperCase() + sentence.slice(1);
}

const icons = {
	toilets: 'toilet',
	bathrooms: 'bath',
	food: 'utensils',
	kitchen: 'sink',
	disabilityFriendly: 'wheelchair',
	outdoorCamping: 'campground',
	indoorCamping: 'warehouse',
	accomodation: 'bed',
	inductions: 'map-signs',
	talksAndTraining: 'comment',
	welfare: 'first-aid',
	affinityGroupFormation: 'hands-helping',
	volunteersNeeded: 'hand-paper',
	familyFriendly: 'baby-carriage',
	internet: 'wifi',
	electricity: 'plug',
	parking: 'parking',
};

export const Facilities = ({ facilities }) => {
	const { t } = useTranslation("ui");

	const facilitiesCount = facilities ? Object.keys(facilities).reduce((acc, key) => (
		acc + (facilities[key] === false || facilities[key] === 'None') ? 0 : 1
	), 0) : 0;

	return (
		<Panel>
			<ul className={styles.facilities}>
				{facilitiesCount ? (
					<p>{t('This event offers no facilities')}.</p>
				)
				: (
					Object.keys(facilities).map(key => {
						const value = facilities[key];
						//if(value === false || value === 'None') return null;
						const text = camelToSentenceCase(key);

						return (
							<li className={styles.facility} key={key}>
								<div className={styles.title}>
									{text}
									<Icon icon={icons[key]} title={text} className={styles.icon} />
								</div>
								<div className={styles.value}>
									{value === true ? t('Yes') : value}
								</div>
							</li>
						)
					})
				)}
			</ul>
		</Panel>
	);
};

export default Facilities;
